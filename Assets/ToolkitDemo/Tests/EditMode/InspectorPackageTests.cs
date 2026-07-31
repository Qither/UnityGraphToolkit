using GraphToolkit.Inspector.Editor;
using NUnit.Framework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolkitDemo.Tests
{
    public sealed class InspectorPackageTests
    {
        [Test]
        public void AssetHelper_ResolvesEmbeddedPackageSourcePath()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string callerPath = Path.Combine(
                projectRoot,
                "Packages",
                "com.graphtoolkit.inspector",
                "Editor",
                "Elements",
                "Frame",
                "Frame.cs");

            string assetPath = AssetHelper.GetAssetPath(callerPath);

            Assert.That(
                assetPath,
                Is.EqualTo("Packages/com.graphtoolkit.inspector/Editor/Elements/Frame/"));
        }

        [Test]
        public void FrameStylesheet_IsAvailableFromEmbeddedPackage()
        {
            StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.graphtoolkit.inspector/Editor/Elements/Frame/Frame.uss");

            Assert.That(stylesheet, Is.Not.Null);
        }
    }
}
