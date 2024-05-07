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
        public void Addition16BitTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x06;

            /*
                LDA #$64
                STA $10
                LDA #$00
                STA $11
                LDA #$58
                STA $12
                LDA #$02
                STA $13
                CLC 
                LDA $10
                ADC $12
                STA $0200
                LDA $11
                ADC $13
                STA $0201
                HLT
            */

            // add 100 + 600 and put the result in memory at 0x200 and 0x201
            memory[0x0600] = 0xA9; // LDA #$64
            memory[0x0601] = 0x64;
            memory[0x0602] = 0x85; // STA $10
            memory[0x0603] = 0x10;
            memory[0x0604] = 0xA9; // LDA #$00
            memory[0x0605] = 0x00;
            memory[0x0606] = 0x85; // STA $11
            memory[0x0607] = 0x11;
            memory[0x0608] = 0xA9; // LDA #$58
            memory[0x0609] = 0x58;
            memory[0x060A] = 0x85; // STA $12
            memory[0x060B] = 0x12;
            memory[0x060C] = 0xA9; // LDA #$02
            memory[0x060D] = 0x02;
            memory[0x060E] = 0x85; // STA $13
            memory[0x060F] = 0x13;
            memory[0x0610] = 0x18; // CLC
            memory[0x0611] = 0xA5; // LDA $10
            memory[0x0612] = 0x10;
            memory[0x0613] = 0x65; // ADC $12
            memory[0x0614] = 0x12;
            memory[0x0615] = 0x8D; // STA $0200
            memory[0x0616] = 0x00;
            memory[0x0617] = 0x02;
            memory[0x0618] = 0xA5; // LDA $11
            memory[0x0619] = 0x11;
            memory[0x061A] = 0x65; // ADC $13
            memory[0x061B] = 0x13;
            memory[0x061C] = 0x8D; // STA $0201
            memory[0x061D] = 0x01;
            memory[0x061E] = 0x02;
            memory[0x061F] = 0x02; // HLT

            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(0xBC, emulator.memory[0x0200]);
            Assert.AreEqual(0x02, emulator.memory[0x0201]);
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
        public void Subtraction16BitTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x06;

            /*
                LDA #$E8
                STA $10
                LDA #$03
                STA $11
                LDA #$2C
                STA $12
                LDA #$01
                STA $13
                SEC 
                LDA $10
                SBC $12
                STA $0200
                LDA $11
                SBC $13
                STA $0201
                HLT
            */

            // subtract 1000 - 300 and put the result in memory at 0x200 and 0x201
            memory[0x0600] = 0xA9; // LDA #$E8
            memory[0x0601] = 0xE8;
            memory[0x0602] = 0x85; // STA $10
            memory[0x0603] = 0x10;
            memory[0x0604] = 0xA9; // LDA #$03
            memory[0x0605] = 0x03;
            memory[0x0606] = 0x85; // STA $11
            memory[0x0607] = 0x11;
            memory[0x0608] = 0xA9; // LDA #$2C
            memory[0x0609] = 0x2C;
            memory[0x060A] = 0x85; // STA $12
            memory[0x060B] = 0x12;
            memory[0x060C] = 0xA9; // LDA #$01
            memory[0x060D] = 0x01;
            memory[0x060E] = 0x85; // STA $13
            memory[0x060F] = 0x13;
            memory[0x0610] = 0x38; // SEC
            memory[0x0611] = 0xA5; // LDA $10
            memory[0x0612] = 0x10;
            memory[0x0613] = 0xE5; // SBC $12
            memory[0x0614] = 0x12;
            memory[0x0615] = 0x8D; // STA $0200
            memory[0x0616] = 0x00;
            memory[0x0617] = 0x02;
            memory[0x0618] = 0xA5; // LDA $11
            memory[0x0619] = 0x11;
            memory[0x061A] = 0xE5; // SBC $13
            memory[0x061B] = 0x13;
            memory[0x061C] = 0x8D; // STA $0201
            memory[0x061D] = 0x01;
            memory[0x061E] = 0x02;
            memory[0x061F] = 0x02; // HLT

            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(0xBC, emulator.memory[0x0200]);
            Assert.AreEqual(0x02, emulator.memory[0x0201]);
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