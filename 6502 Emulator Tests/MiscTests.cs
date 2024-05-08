namespace _6502_Emulator_Tests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public void NOP()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x06;

            /*
                NOP
                HLT
            */

            memory[0x600] = 0xEA; // NOP
            memory[0x601] = 0x02; // HLT



            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(0, emulator.memory[0x0200]);
            Assert.AreEqual(0b00110000, emulator.status);
        }
    }
}
