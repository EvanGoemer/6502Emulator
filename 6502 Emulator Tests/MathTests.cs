namespace _6502_Emulator_Tests
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void AdditonTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x00;

            /*
                LDA #2
                CLC
                ADC #2
                STA $0200
                HLT
            */

            // add 2 + 2 and put the result in memory at 0x200
            memory[0x0000] = 0xA9; // LDA #2
            memory[0x0001] = 0x02;
            memory[0x0002] = 0x18; // CLC
            memory[0x0003] = 0x69; // ADC #2
            memory[0x0004] = 0x02;
            memory[0x0005] = 0x8D; // STA $0200
            memory[0x0006] = 0x00;
            memory[0x0007] = 0x02;
            memory[0x0008] = 0x02; // HLT

            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(4, emulator.memory[0x0200]);
            Assert.AreEqual(0b00110000, emulator.status);
        }

        [TestMethod]
        public void AdditonOverflowZeroTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x00;

            /*
                LDA #255
                CLC
                ADC #1
                STA $0200
                HLT
            */

            // add 255 + 1 and put the result in memory at 0x200
            memory[0x0000] = 0xA9; // LDA #255
            memory[0x0001] = 0xFF;
            memory[0x0002] = 0x18; // CLC
            memory[0x0003] = 0x69; // ADC #2
            memory[0x0004] = 0x01;
            memory[0x0005] = 0x8D; // STA $0200
            memory[0x0006] = 0x00;
            memory[0x0007] = 0x02;
            memory[0x0008] = 0x02; // HLT

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
            Assert.AreEqual(0b00110011, emulator.status);
        }

        [TestMethod]
        public void AdditonNegativeTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x00;

            /*
                LDA #200
                CLC
                ADC #1
                STA $0200
                HLT
            */

            // add -56 + 1 and put the result in memory at 0x200
            memory[0x0000] = 0xA9; // LDA #200
            memory[0x0001] = 0xC8;
            memory[0x0002] = 0x18; // CLC
            memory[0x0003] = 0x69; // ADC #1
            memory[0x0004] = 0x01;
            memory[0x0005] = 0x8D; // STA $0200
            memory[0x0006] = 0x00;
            memory[0x0007] = 0x02;
            memory[0x0008] = 0x02; // HLT

            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(201, emulator.memory[0x0200]);
            Assert.AreEqual(0b10110000, emulator.status);
        }

        [TestMethod]
        public void SubtractionTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x00;

            /*
                LDA #4
                SEC
                SBC #3
                STA $0200
                HLT
            */

            // subtract 4 - 3 and put the result in memory at 0x200
            memory[0x0000] = 0xA9; // LDA #4
            memory[0x0001] = 0x04;
            memory[0x0002] = 0x38; // SEC
            memory[0x0003] = 0xE9; // SBC #3
            memory[0x0004] = 0x03;
            memory[0x0005] = 0x8D; // STA $0200
            memory[0x0006] = 0x00;
            memory[0x0007] = 0x02;
            memory[0x0008] = 0x02; // HLT

            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(1, emulator.memory[0x0200]);
            Assert.AreEqual(0b00110001, emulator.status);
        }

        [TestMethod]
        public void SubtractionUnderflowNegativeTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x00;

            /*
                LDA #0
                SEC
                SBC #1
                STA $0200
                HLT
            */

            // subtract 0 - 1 and put the result in memory at 0x200
            memory[0x0000] = 0xA9; // LDA #0
            memory[0x0001] = 0x00;
            memory[0x0002] = 0x38; // SEC
            memory[0x0003] = 0xE9; // SBC #1
            memory[0x0004] = 0x01;
            memory[0x0005] = 0x8D; // STA $0200
            memory[0x0006] = 0x00;
            memory[0x0007] = 0x02;
            memory[0x0008] = 0x02; // HLT

            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(255, emulator.memory[0x0200]);
            Assert.AreEqual(0b10110000, emulator.status);
        }

        [TestMethod]
        public void SubtractionZeroTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x00;

            /*
                LDA #1
                SEC
                SBC #1
                STA $0200
                HLT
            */

            // subtract 1 - 1 and put the result in memory at 0x200
            memory[0x0000] = 0xA9; // LDA #1
            memory[0x0001] = 0x01;
            memory[0x0002] = 0x38; // SEC
            memory[0x0003] = 0xE9; // SBC #1
            memory[0x0004] = 0x01;
            memory[0x0005] = 0x8D; // STA $0200
            memory[0x0006] = 0x00;
            memory[0x0007] = 0x02;
            memory[0x0008] = 0x02; // HLT

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
            Assert.AreEqual(0b00110011, emulator.status);
        }
    }
}