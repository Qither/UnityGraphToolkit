using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace ToolkitDemo.Tests
{
    public sealed class ToolkitDemoPlayModeTests
    {
        [UnityTest]
        public IEnumerator DemoScene_RunsBehaviorAndRedDotInteraction()
        {
            AsyncOperation load = SceneManager.LoadSceneAsync("ToolkitDemo", LoadSceneMode.Single);
            while (!load.isDone)
            {
                yield return null;
            }

            yield return null;
            ToolkitDemoController controller = Object.FindFirstObjectByType<ToolkitDemoController>();
            Assert.That(controller, Is.Not.Null);
            Assert.That(controller.BehaviorRunCount, Is.EqualTo(1));
            Assert.That(controller.RedDotValue, Is.Zero);

            controller.IncrementPrimary();
            Assert.That(controller.RedDotValue, Is.EqualTo(1));
            Assert.That(controller.redDotIndicator.activeSelf, Is.True);

            controller.ToggleSystemAvailability();
            Assert.That(controller.RedDotValue, Is.Zero);
            Assert.That(controller.redDotIndicator.activeSelf, Is.False);
        }
    }
}
