#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowState.Tests.EditMode
{
    // Candidate-only diagnostics. Never opens a Scene, starts Play, or saves assets.
    public sealed class Phase3RestoreReadOnlyTests
    {
        private const string CandidateRoot = "C:/Users/Admin/AppData/Local/Temp/FlowState_Phase3_20261120_01/Project";
        private const string InputPath = "C:/Unity/Unity_Flow_State/AI/90_Tasks/Prototype_10/20261125_15_Phase3Step8ReadOnlyInput.json";
        private const string InputSha256 = "e991d38eb86f96cab9154c8980e2071f867c35d71721176efe795beeb857ea3b";
        private const string OutputRoot = "C:/Users/Admin/AppData/Local/Temp/FlowState_Phase3_20261120_01/Outputs";
        private readonly HashSet<string> _checkedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Input _input;
        private Report _report;
        private bool _ready;

        [Serializable] public sealed class Pin { public string path; public string sha256; }
        [Serializable] public sealed class Reference
        {
            public string path; public string guid; public string fileID;
            public int line; public string group; public string owner;
        }
        [Serializable] public sealed class Input
        {
            public int schemaVersion; public string candidateRoot; public string editorVersion;
            public Pin[] protectedFiles; public string[] profiles; public string[] shaderGraphs;
            public string[] prefabs; public Reference[] unresolvedReferences; public string[] types;
        }
        [Serializable] public sealed class Observation
        {
            public string path; public string subject; public string value;
        }
        [Serializable] public sealed class Report
        {
            public string test; public string utc; public string project; public string editor;
            public string runnerOutcomeBeforeTearDown;
            public bool approvalGranted;
            public List<Observation> observations = new List<Observation>();
            public List<string> assertionIssues = new List<string>();
            public List<string> reviewRequired = new List<string>();
        }

        [SetUp]
        public void CheckCandidateAndPinnedInputs()
        {
            _ready = false;
            _checkedPaths.Clear();
            string actualRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            Assert.That(SamePath(actualRoot, CandidateRoot), Is.True, "Run only in the named restore candidate.");
            Assert.That(Application.unityVersion, Is.EqualTo("6000.3.5f2"));
            Assert.That(Application.isPlaying, Is.False, "EditMode only.");
            RejectLinks(InputPath);
            Assert.That(Sha256(InputPath), Is.EqualTo(InputSha256), "Reviewed input changed.");
            _input = JsonUtility.FromJson<Input>(File.ReadAllText(InputPath, Encoding.UTF8));
            Assert.That(_input.schemaVersion, Is.EqualTo(1));
            Assert.That(SamePath(_input.candidateRoot, CandidateRoot), Is.True);
            Assert.That(_input.editorVersion, Is.EqualTo(Application.unityVersion));
            _report = new Report
            {
                test = TestContext.CurrentContext.Test.Name,
                utc = DateTime.UtcNow.ToString("o"), project = actualRoot, editor = Application.unityVersion,
                approvalGranted = false
            };
            foreach (Pin pin in _input.protectedFiles)
                Assert.That(Sha256(ProtectedPath(pin.path)), Is.EqualTo(pin.sha256), "Pre-test file changed: " + pin.path);
            RejectLinks(OutputRoot);
            Assert.That(Directory.Exists(OutputRoot), Is.True);
            _ready = true;
        }

        [Test]
        public void RequiredTypes_AreAvailableWithoutStartingSystems()
        {
            foreach (string name in _input.types)
            {
                Type type = Type.GetType(name, false);
                Observe("assembly", name, type == null ? "not found" : type.AssemblyQualifiedName);
                Check(type != null, "Required compiled type not found: " + name);
            }
            Observe("modules", "native types", typeof(Collider).FullName + "; " + typeof(Camera).FullName + "; " + typeof(Font).FullName);
            Review("U3: compare current Console/import status with the 37 static orphan records; type availability alone does not dismiss them.");
        }

        [Test]
        public void ActiveUrpAssets_LoadAndReportVolumeAndDebugReferences()
        {
            Object pipeline = Load("Assets/Settings/PC_RPAsset.asset");
            if (pipeline != null)
            {
                using (var serialized = new SerializedObject(pipeline))
                    InspectArray(serialized.FindProperty("m_RendererDataList"), "PC_RPAsset.rendererDataList");
            }
            Object renderer = Load("Assets/Settings/PC_Renderer.asset");
            if (renderer != null)
            {
                using (var serialized = new SerializedObject(renderer))
                    InspectArray(serialized.FindProperty("m_RendererFeatures"), "PC_Renderer.features");
            }
            Load("Assets/Settings/UniversalRenderPipelineGlobalSettings.asset");
            foreach (string path in _input.profiles)
            {
                Object profile = Load(path);
                if (profile == null) continue;
                using (var serialized = new SerializedObject(profile))
                    InspectArray(serialized.FindProperty("components"), path + ".components");
                Observe(path, "subassetCount", AssetDatabase.LoadAllAssetsAtPath(path).Length.ToString());
                Review(path + ": compare imported component identities with raw YAML, including existing test/null-script entries. VolumeProfile.OnEnable removes null entries in memory; do not save it.");
            }
            InspectKnownReferences(false);
        }

        [Test]
        public void ShaderGraphs_ReportActualImporterAndCompilerMessages()
        {
            foreach (string path in _input.shaderGraphs)
            {
                AssetImporter importer = AssetImporter.GetAtPath(path);
                Observe(path, "importer", importer == null ? "null" : importer.GetType().FullName);
                Check(importer != null, "No importer for " + path);
                Object main = AssetDatabase.LoadMainAssetAtPath(path);
                Observe(path, "mainAsset", Describe(main));
                Shader shader = main as Shader;
                if (shader == null)
                {
                    Review(path + ": no Shader main object; current importer/Console result requires review.");
                    continue;
                }
                Observe(path, "shaderSupported", shader.isSupported.ToString());
                foreach (var message in ShaderUtil.GetShaderMessages(shader))
                {
                    string detail = message.severity + ": " + message.message;
                    Observe(path, "compilerMessage", detail);
                    Review(path + ": " + detail);
                }
            }
            InspectKnownReferences(true);
            Review("U2: unsupported HDRP/unused graphs and the known texture references require an explicit scope decision; a green diagnostic test is not shader-health approval.");
        }

        [Test]
        public void TmpResources_LoadFontAtlasSpriteAndMaterialSubassets()
        {
            Object settings = Load("Assets/TextMesh Pro/Resources/TMP Settings.asset");
            if (settings != null)
            {
                using (var serialized = new SerializedObject(settings))
                {
                    Object font = InspectReference(serialized.FindProperty("m_defaultFontAsset"), "TMP.defaultFont");
                    Object sprite = InspectReference(serialized.FindProperty("m_defaultSpriteAsset"), "TMP.defaultSprite");
                    InspectReference(serialized.FindProperty("m_defaultStyleSheet"), "TMP.defaultStyleSheet");
                    if (font != null)
                    {
                        using (var fontData = new SerializedObject(font))
                        {
                            InspectArray(fontData.FindProperty("m_AtlasTextures"), "TMP.fontAtlas");
                            InspectMaterial(fontData.FindProperty("m_Material"), "TMP.fontMaterial");
                        }
                    }
                    if (sprite != null)
                    {
                        using (var spriteData = new SerializedObject(sprite))
                            InspectReference(spriteData.FindProperty("spriteSheet"), "TMP.spriteSheet");
                    }
                }
            }
            Object fallback = Load("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset");
            if (fallback != null)
            {
                using (var serialized = new SerializedObject(fallback))
                    InspectReference(serialized.FindProperty("m_SourceFontFile"), "TMP.fallbackSourceFont");
            }
            Review("U4: actual text/sprite appearance still needs the isolated visual check; this test never renders or populates dynamic font atlases.");
        }

        [Test]
        public void PrefabAssets_LoadAuthoringAndSerializedAnchorsWithoutInstantiation()
        {
            foreach (string path in _input.prefabs)
            {
                GameObject prefab = Load(path) as GameObject;
                Check(prefab != null, "Prefab main object is not a GameObject: " + path);
                if (prefab == null) continue;
                Component authoring = null;
                foreach (Transform child in prefab.GetComponentsInChildren<Transform>(true))
                    foreach (Component component in child.GetComponents<Component>())
                    {
                        Check(component != null, "Missing script on prefab " + path + "/" + child.name);
                        if (component != null && component.GetType().FullName == "FlowState.Runtime.Features.InfinitePatternAuthoring")
                            authoring = component;
                    }
                Check(authoring != null, "InfinitePatternAuthoring missing: " + path);
                if (authoring == null) continue;
                using (var serialized = new SerializedObject(authoring))
                    foreach (string field in new[] { "_geometryRoot", "_startAnchor", "_endAnchor", "_advanceBoundaryPoint", "_collectibleRoot" })
                        InspectReference(serialized.FindProperty(field), path + ":" + field);
            }
            Review("U5: no Scene was opened and no prefab was instantiated; actual isolated Scene/runtime/UI linkage is a separate follow-up.");
        }

        private void InspectKnownReferences(bool graphs)
        {
            foreach (Reference reference in _input.unresolvedReferences)
            {
                if ((reference.group == "tmp-texture") != graphs) continue;
                string resolved = AssetDatabase.GUIDToAssetPath(reference.guid);
                string value = "GUID has no AssetDatabase path";
                if (!string.IsNullOrEmpty(resolved))
                {
                    value = "path=" + resolved + "; localID not found";
                    foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(resolved))
                    {
                        if (asset == null) continue;
                        string guid;
                        long localID;
                        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out localID) &&
                            guid == reference.guid && localID.ToString(System.Globalization.CultureInfo.InvariantCulture) == reference.fileID)
                            value = Describe(asset);
                    }
                }
                string subject = reference.guid + "/" + reference.fileID + " at line " + reference.line;
                Observe(reference.path, subject, value);
                Review(reference.path + ": " + subject + " -> " + value);
            }
        }

        private Object Load(string path)
        {
            Object asset = AssetDatabase.LoadMainAssetAtPath(path);
            Observe(path, "mainAsset", Describe(asset));
            Check(asset != null, "Required main asset did not load: " + path);
            return asset;
        }

        private Object InspectReference(SerializedProperty property, string subject)
        {
            Check(property != null, "Serialized field not found: " + subject);
            if (property == null) return null;
            Check(property.propertyType == SerializedPropertyType.ObjectReference, "Not an object reference: " + subject);
            if (property.propertyType != SerializedPropertyType.ObjectReference) return null;
            Object value = property.objectReferenceValue;
            Observe(subject, property.propertyPath, Describe(value));
            Check(value != null, "Required imported reference is null: " + subject);
            return value;
        }

        private void InspectArray(SerializedProperty property, string subject)
        {
            Check(property != null && property.isArray, "Array field not found: " + subject);
            if (property == null || !property.isArray) return;
            Observe(subject, "count", property.arraySize.ToString());
            Check(property.arraySize > 0, "Required array is empty: " + subject);
            for (int i = 0; i < property.arraySize; i++)
                InspectReference(property.GetArrayElementAtIndex(i), subject + "[" + i + "]");
        }

        private void InspectMaterial(SerializedProperty property, string subject)
        {
            Material material = InspectReference(property, subject) as Material;
            Check(material != null, "Expected material: " + subject);
            if (material == null) return;
            Check(material.shader != null, "Material has no shader: " + subject);
            if (material.shader == null) return;
            Observe(subject, "shader", material.shader.name);
            Check(material.shader.isSupported, "Current material shader unsupported: " + subject);
            foreach (var message in ShaderUtil.GetShaderMessages(material.shader))
                Review(subject + ": " + message.severity + ": " + message.message);
        }

        private static string Describe(Object value)
        {
            if (value == null) return "null";
            string guid;
            long localID;
            bool identified = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(value, out guid, out localID);
            return value.name + "; " + value.GetType().FullName + "; " + AssetDatabase.GetAssetPath(value) +
                (identified ? "; " + guid + "/" + localID : "; no persistent localID");
        }

        private void Observe(string path, string subject, string value)
        {
            _report.observations.Add(new Observation { path = path, subject = subject, value = value });
        }

        private void Check(bool condition, string message)
        {
            if (!condition) _report.assertionIssues.Add(message);
        }

        private void Review(string message) { _report.reviewRequired.Add(message); }

        [TearDown]
        public void PreserveInputsAndWriteNewDiagnosticReport()
        {
            if (!_ready) return;
            _checkedPaths.Clear();
            foreach (Pin pin in _input.protectedFiles)
            {
                string file = ProtectedPath(pin.path);
                Check(File.Exists(file) && Sha256(file) == pin.sha256, "Post-test input changed: " + pin.path);
            }
            _report.runnerOutcomeBeforeTearDown = TestContext.CurrentContext.Result.Outcome.Status.ToString();
            string filename = "Step8ReadOnly_" + _report.test + "_" + DateTime.UtcNow.Ticks + "_" + Guid.NewGuid().ToString("N") + ".json";
            string output = Path.Combine(OutputRoot, filename);
            RejectLinks(OutputRoot);
            using (var stream = new FileStream(output, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                writer.Write(JsonUtility.ToJson(_report, true));
            TestContext.Out.WriteLine("Read-only report: " + output + "; review items=" + _report.reviewRequired.Count);
            Assert.That(_report.assertionIssues, Is.Empty, string.Join("\n", _report.assertionIssues));
        }

        private string ProtectedPath(string relative)
        {
            Assert.That(Path.IsPathRooted(relative), Is.False);
            string full = Path.GetFullPath(Path.Combine(CandidateRoot, relative));
            Assert.That(full.StartsWith(Path.GetFullPath(CandidateRoot) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase), Is.True);
            RejectLinks(full);
            return full;
        }

        private void RejectLinks(string file)
        {
            string cursor = Path.GetFullPath(file);
            while (!string.IsNullOrEmpty(cursor) && _checkedPaths.Add(cursor))
            {
                Assert.That((File.GetAttributes(cursor) & FileAttributes.ReparsePoint) == 0, Is.True, "Link/junction: " + cursor);
                cursor = Path.GetDirectoryName(cursor);
            }
        }

        private static bool SamePath(string a, string b)
        {
            return string.Equals(Path.GetFullPath(a).TrimEnd('\\', '/'), Path.GetFullPath(b).TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase);
        }

        private static string Sha256(string file)
        {
            using (SHA256 algorithm = SHA256.Create())
            using (FileStream stream = File.OpenRead(file))
                return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }
    }
}
#endif
