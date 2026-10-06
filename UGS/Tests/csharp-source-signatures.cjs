// Limited declaration audit, not a C# compiler. Preserves nested type scopes and overloads.
function duplicateMembers(source) {
  const plain = source.replace(/"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|\/\/[^\r\n]*|\/\*[\s\S]*?\*\//g,
    text => " ".repeat(text.length));
  const scopes = [];
  for (const match of plain.matchAll(/\b(?:class|struct|interface)\s+(\w+)[^{};]*\{/g)) {
    let depth = 1, end = match.index + match[0].length;
    for (; end < plain.length && depth; end++) {
      if (plain[end] === "{") depth++;
      if (plain[end] === "}") depth--;
    }
    scopes.push({ name: match[1], start: match.index, end });
  }
  const signatures = new Set(), duplicates = [];
  // Simple single-field declarations, including initialized DTO fields.
  // This limited audit is not a C# compiler.
  const fields = /\b(?:public|private|internal|protected)\s+(?:(?:static|readonly|const|volatile|new)\s+)*[\w.]+(?:<[\w.,<>\[\] \t]+>)?(?:\[\])?\s+(\w+)\s*(?:=(?!>)[^;{}\r\n]*)?;/g;
  for (const match of plain.matchAll(fields)) {
    const types = scopes.filter(scope => match.index > scope.start && match.index < scope.end);
    if (!types.length) continue;
    const signature = types.map(type => type.name + "@" + type.start).join("/") + "/field:" + match[1];
    if (signatures.has(signature)) duplicates.push(signature);
    signatures.add(signature);
  }
  const methods = /\b(?:public|private|internal|protected)\s+(?:(?:static|async|override|virtual|sealed|new|extern|partial)\s+)*(?:[\w.]+(?:<[^{};\r\n]+>)?(?:\[\])?)\s+(\w+)\s*(<[^{}();]+>)?\s*\(([^()]*)\)/g;
  for (const match of plain.matchAll(methods)) {
    if (match[0].slice(0, match[0].indexOf("(")).includes("=")) continue;
    const types = scopes.filter(scope => match.index > scope.start && match.index < scope.end);
    if (!types.length) continue;
    const generic = match[2] ? match[2].slice(1, -1).split(",").map(value => value.trim()) : [];
    let params = [], part = "", depth = 0;
    for (const char of match[3] + ",") {
      if (char === "<" || char === "[") depth++;
      if (char === ">" || char === "]") depth--;
      if (char === "," && depth === 0) { if (part.trim()) params.push(part); part = ""; }
      else part += char;
    }
    params = params.map(value => {
      let type = value.split("=")[0].trim().replace(/\s+\w+\s*$/, "")
        .replace(/\b(?:params|this)\s+/g, "").replace(/\b(?:ref|out|in)\s+/g, "byref ").replace(/\s+/g, "");
      for (let i = 0; i < generic.length; i++) type = type.replace(new RegExp("\\b" + generic[i] + "\\b", "g"), "!" + i);
      return type;
    });
    const signature = types.map(type => type.name + "@" + type.start).join("/") + "/" +
      match[1] + "`" + generic.length + "(" + params.join(",") + ")";
    if (signatures.has(signature)) duplicates.push(signature);
    signatures.add(signature);
  }
  return duplicates;
}
module.exports = { duplicateMembers };
