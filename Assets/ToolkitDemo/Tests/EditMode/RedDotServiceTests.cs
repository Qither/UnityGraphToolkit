using NUnit.Framework;
using RedDotSystem.Runtime;
using ToolkitDemo.RedDot.Generated;
using ToolkitDemo.RedDotDemo;

namespace ToolkitDemo.Tests
{
    public sealed class RedDotServiceTests
    {
        [Test]
        public void Service_AggregatesMultiNodesAndHonorsSystemGate()
        {
            bool systemAvailable = true;
            RedDotService service = new RedDotService(
                new RedDotExecuteNode(),
                ToolkitSerializationTests.CreateRedDotData(),
                _ => systemAvailable);
            int observedValue = -1;
            service.StaticBind("ROOT_Inbox", node => observedValue = node.Data.nodeValue);

            DemoRedDotState.PrimaryCount = 2;
            DemoRedDotState.SecondaryCount = 3;
            service.ExecuteNodeDataDirty("ROOT_Inbox_Rewards_ALPHA");
            service.ExecuteNodeDataDirty("ROOT_Inbox_Rewards_BETA");
            service.Update();
            Assert.That(observedValue, Is.EqualTo(5));

            int dynamicValue = -1;
            service.DynamicBind("ROOT_Inbox_Rewards", node => dynamicValue = node.Data.nodeValue, "alpha");
            Assert.That(dynamicValue, Is.EqualTo(2));

            systemAvailable = false;
            service.ExecuteNodeDataDirty("ROOT_Inbox_Rewards_ALPHA");
            service.ExecuteNodeDataDirty("ROOT_Inbox_Rewards_BETA");
            service.Update();
            Assert.That(observedValue, Is.Zero);

            Assert.DoesNotThrow(service.Dispose);
        }
    }
}
