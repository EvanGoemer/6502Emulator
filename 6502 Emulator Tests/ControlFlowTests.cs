﻿namespace _6502_Emulator_Tests
{
    [TestClass]
    public class ControlFlowTests
    {
        [TestMethod]
        public void BNETest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x06;

            /*
                LDA #$08
                LDX #$07
                CLC
                LOOP:
                    ADC #$08
                    DEX 
                BNE LOOP
                STA $0200
                HLT
            */

            memory[0x600] = 0xA9; // LDA #$08
            memory[0x601] = 0x08;
            memory[0x602] = 0xA2; // LDX #$07
            memory[0x603] = 0x07;
            memory[0x604] = 0x18; // CLC
                                  // LOOP:
            memory[0x605] = 0x69; // ADC #$08
            memory[0x606] = 0x08;
            memory[0x607] = 0xCA; // DEX
            memory[0x608] = 0xD0; // BNE LOOP
            memory[0x609] = 0xFB;
            memory[0x60A] = 0x8D; // STA $0200
            memory[0x60B] = 0x00;
            memory[0x60C] = 0x02;
            memory[0x60D] = 0x02; // HLT



            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(64, emulator.memory[0x0200]);
            Assert.AreEqual(0b00110010, emulator.status);
        }

        [TestMethod]
        public void BEQJMPTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x06;

            /*
                LDA #$08
                LDX #$07
                CLC
                LOOP:
                    ADC #$08
                    DEX
                    BEQ EXIT
                    JMP LOOP
                EXIT:
                    STA $0200
                HLT
            */

            memory[0x600] = 0xA9; // LDA #$08
            memory[0x601] = 0x08;
            memory[0x602] = 0xA2; // LDX #$07
            memory[0x603] = 0x07;
            memory[0x604] = 0x18; // CLC
                                  // LOOP:
            memory[0x605] = 0x69; // ADC #$08
            memory[0x606] = 0x08;
            memory[0x607] = 0xCA; // DEX
            memory[0x608] = 0xF0; // BEQ EXIT
            memory[0x609] = 0x03;
            memory[0x60A] = 0x4C; // JMP LOOP
            memory[0x60B] = 0x05;
            memory[0x60C] = 0x06; 
                                  // EXIT:
            memory[0x60D] = 0x8D; // STA $0200
            memory[0x60E] = 0x00;
            memory[0x60F] = 0x02;
            memory[0x610] = 0x02; // HLT



            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(64, emulator.memory[0x0200]);
            Assert.AreEqual(0b00110010, emulator.status);
        }

        [TestMethod]
        public void JSRRTSTest()
        {
            byte[] memory = new byte[64 * 1024];
            memory[0xFFFC] = 0x00;
            memory[0xFFFD] = 0x06;

            /*
                LDA #12
                LDX #12
                NOP
                JSR MULTIPY
                STA $0200
		        HLT
		
                MULTIPY:
		            STA $00
                    DEX
                    CLC
                    LOOP:
                        ADC $00
                        DEX 
                    BNE LOOP
                    RTS
            */

            memory[0x600] = 0xA9; // LDA #12
            memory[0x601] = 0x0C;
            memory[0x602] = 0xA2; // LDX #12
            memory[0x603] = 0x0C;
            memory[0x604] = 0xEA; // NOP
            memory[0x605] = 0x20; // JSR MULTIPY
            memory[0x606] = 0x0C;
            memory[0x607] = 0x06;
            memory[0x608] = 0x8D; // STA $0200
            memory[0x609] = 0x00;
            memory[0x60A] = 0x02;
            memory[0x60B] = 0x02; // HLT
                                  // MULTIPY:
            memory[0x60C] = 0x85; // STA $00
            memory[0x60D] = 0x00;
            memory[0x60E] = 0xCA; // DEX
            memory[0x60F] = 0x18; // CLC
                                  // LOOP:
            memory[0x610] = 0x65; // ADC $00
            memory[0x611] = 0x00;
            memory[0x612] = 0xCA; // DEX
            memory[0x613] = 0xD0; // BNE LOOP
            memory[0x614] = 0xFB;
            memory[0x615] = 0x60; // RTS

            Emulator6502 emulator = new Emulator6502(memory);
            while (emulator.isRunning)
            {
                emulator.executeNextInstruction();

                if (emulator.debug)
                {
                    emulator.printDebugInfo();
                }
            }

            Assert.AreEqual(144, emulator.memory[0x0200]);
            Assert.AreEqual(0b00110010, emulator.status);
        }
    }
}
