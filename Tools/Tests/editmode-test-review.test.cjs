"use strict";
const assert = require("node:assert/strict");
const fs = require("node:fs"), path = require("node:path");
const { methods, rows, groups, header, rowText, ledgerRows, removed } = require("./editmode-test-review.cjs");
assert.equal(methods.length, 605);
assert.equal(rows.length, 887);
assert.equal(new Set(methods.map(test => test.fixture)).size, 52);
assert.equal(methods.filter(test => test.cases[0] === "[Test]").length, 505);
assert.equal(rows.filter(test => test.input.startsWith("[TestCase(")).length, 382);
assert.equal(new Set(rows.map(test => test.category)).size, 10);
assert.equal(groups.reduce((sum, group) => sum + rows.filter(row => row.category === group[0]).length, 0), 887);
const expectedCounts = { "유지 권고": 870, "검토 조치 적용": 14, "이름 정정 적용": 2, "통합 보강 적용": 1 };
for (const [verdict, count] of Object.entries(expectedCounts)) assert.equal(rows.filter(row => row.verdict === verdict).length, count, verdict);
const read = name => fs.readFileSync(path.resolve(__dirname, "../../AI/90_Tasks/Prototype_8/" + name), "utf8").replace(/\r\n/g, "\n").trimEnd();
assert.equal(read("20261006_04_EditModeTestActions.csv"), [header, ...ledgerRows.map(rowText)].join("\n"));
assert.equal(ledgerRows.length, 43);
assert.equal(new Set(rows.map(row => row.file + ":" + row.method + ":" + row.index)).size, 887);
assert.equal(removed.length, 26);
assert.equal(removed.filter(row => row.fixture === "ScoreCalculatorTests").length, 23);
for (const file of ["Assets/Scripts/Runtime/Features/ScoreCalculator.cs", "Assets/Tests/EditMode/ScoreCalculatorTests.cs"])
  for (const suffix of ["", ".meta"]) assert.ok(!fs.existsSync(path.resolve(__dirname, "../..", file + suffix)), file + suffix);
for (const row of removed) assert.ok(!methods.some(test => test.fixture === row.fixture && test.method === row.method));
// The initial 909-row review is retained as history, not overwritten with current line numbers.
const baseline = read("20261006_02_EditModeTestAudit_Cases.csv").split("\n").slice(1).map(line =>
  [...line.matchAll(/"((?:""|[^"])*)"(?:,|$)/g)].map(match => match[1].replace(/""/g, '"')));
assert.equal(baseline.length, 909);
const baselineKeys = new Map(baseline.map(fields => [fields[2] + "." + fields[3] + ":" + fields[4], fields]));
for (const row of rows.filter(test => test.verdict === "유지 권고")) {
  const original = baselineKeys.get(row.fixture + "." + row.method + ":" + row.input.replace(/[\r\n]+/g, " "));
  assert.ok(original, row.method + " unchanged case identity");
  assert.equal(row.assertions, Number(original[9]), row.method + " unchanged assertion count");
}
const find = (fixture, method) => {
  const test = methods.find(test => test.fixture === fixture && test.method === method);
  assert.ok(test, fixture + "." + method); return test;
};
const difficulty = find("GameNavigationStateTests", "ShouldShowDifficulty_RequiresDevelopmentContextAndSetting");
assert.deepEqual(difficulty.cases, [
  "[TestCase(false, false, false, false)]", "[TestCase(false, false, true, false)]",
  "[TestCase(false, true, false, false)]", "[TestCase(false, true, true, true)]",
  "[TestCase(true, false, false, false)]", "[TestCase(true, false, true, true)]",
  "[TestCase(true, true, false, false)]", "[TestCase(true, true, true, true)]"
]);
assert.ok(!difficulty.body.includes("bool expected ="));
const sequence = find("InfinitePatternSelectionStateTests", "SameCatalogSeedAndRequests_ProducesSameSequence");
for (const receiver of ["first", "second"]) {
  assert.ok(sequence.body.includes("Assert.That(" + receiver + ".StartRun("));
  assert.ok(sequence.body.includes("Assert.That(" + receiver + ".TrySelectNext("));
}
assert.ok(sequence.body.includes("Is.Not.Null.And.Not.Empty"));
const repeat = find("InfinitePatternSelectionStateTests", "TrySelectNext_WhenAlternativeExists_RespectsRepeatLimit");
assert.ok(repeat.body.includes("Assert.That(state.TrySelectNext("));
assert.ok(repeat.body.includes('Is.EqualTo("Flat").Or.EqualTo("Other")'));
assert.ok(!methods.some(test => test.body.includes("Is.AnyOf(")), "Unsupported NUnit constraint must not return");
const rebase = find("WorldRebaseStateTests", "RepeatedRebase_PreservesScoreAndDifficultyContinuity");
for (const phase of ["First", "Second"])
  for (const metric of ["distance", "difficulty", "score"])
    assert.ok(rebase.body.includes("Is.EqualTo(" + metric + "Before" + phase + "Rebase)"));
const timeout = find("OnlineRecordRepositoryTests", "SubmissionTimeout_RetryRetainsOriginalPendingAcrossRestart");
for (const fragment of ["await coordinator.RetryPendingAsync()", "restored.TryLoad(", "restored.CreatePendingSnapshot()[0].SubmissionId", "coordinator.TryGetTerminalResult(", "milliseconds => Task.CompletedTask"])
  assert.ok(timeout.body.includes(fragment), fragment);
// Same body is not a semantic duplicate when literal inputs/expected results differ.
const valid = methods.find(test => test.method === "FormatStageElapsedTime_ValidTime_ReturnsModeText");
const invalid = methods.find(test => test.method === "FormatStageElapsedTime_InvalidTime_ReturnsPlaceholder");
assert.equal(valid.hash, invalid.hash);
assert.notDeepEqual(valid.cases, invalid.cases);
assert.equal(valid.verdict, "유지 권고");
assert.equal(invalid.verdict, "유지 권고");
console.log("PASS / STATIC_AUDIT_ACTIONS / Fixtures=52 / Methods=605 / Cases=887 / HistoricalCases=909 / Actions=43 / UnityNotRun=True");
