using acmi_interpreter;

namespace acmi_tests
{
    [TestClass]
    public class ACMIMessageTests
    {
        [TestMethod]
        public void TestSegmentGeneration()
        {
            string line = @"0,Briefing=Here is a text value\, which contains an escaped comma in it!,Name=somethingelse";

            ACMIMessage message = new ACMIMessage(line);

            Assert.IsTrue(message.IsGlobal);
            Assert.IsTrue(message.Segments.Length == 3);
        }
    }
}