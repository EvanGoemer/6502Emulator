public class Program
{
    public static void Main()
    {
        byte[] memory = new byte[64 * 1024];
        memory[0xFFFC] = 0x00;
        memory[0xFFFD] = 0x00;

        /*
            LDA #2
            CLC
            ADC #2
            STA $0200

            SEC
            SBC #3
            STA $0201

            HLT
        */

        // add 2+2 and put the result in memory at 0x200
        memory[0x0000] = 0xA9; // LDA #2
        memory[0x0001] = 0x02;
        memory[0x0002] = 0x18; // CLC
        memory[0x0003] = 0x69; // ADC #2
        memory[0x0004] = 0x02;
        memory[0x0005] = 0x8D; // STA $0200
        memory[0x0006] = 0x00;
        memory[0x0007] = 0x02;

        // subtract 3 from result of previous operation and put the result in memory at 0x201
        memory[0x0008] = 0x38; // SEC
        memory[0x0009] = 0xE9; // SBC #3
        memory[0x000A] = 0x03;
        memory[0x000B] = 0x8D; // STA $0201
        memory[0x000C] = 0x01;
        memory[0x000D] = 0x02;

        // execute a NOP
        memory[0x000E] = 0xEA;

        // stop program
        memory[0x000F] = 0x02; // HLT

        Emulator6502 emulator = new Emulator6502(memory);
        while (emulator.isRunning)
        {
            emulator.executeNextInstruction();

            if(emulator.debug)
            { 
                emulator.printDebugInfo(); 
            }
        }
    }
}

public class Emulator6502
{
    public bool debug = true;
    public bool isRunning = true;

    public byte[] memory = new byte[64 * 1024];
    public ushort pc;
    public byte sp;
    public byte a, x, y;
    public byte status;

    public Emulator6502(byte[] program)
    {
        memory = program;
        pc = (ushort)(memory[0xFFFD] << 8 | memory[0xFFFC]);
        sp = 0xFF;
        status = 0b00110000;
    }

    public void executeNextInstruction()
    {
        byte instuction = memory[pc];
        switch (instuction)
        {
            case 0x02: // HLT
                if(debug)
                {
                    printInstructionInfo();
                }

                isRunning = false;
                break;
            case 0x18: // CLC
                if (debug)
                {
                    printInstructionInfo();
                }

                setCarryFlag(false);
                pc++;

                break;
            case 0x38: // SEC
                if (debug)
                {
                    printInstructionInfo();
                }

                setCarryFlag(true);
                pc++;

                break;
            case 0x69: // ADC #immediate
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                byte value = memory[pc];
                pc++;
                ushort result = (ushort)(a + value + (carryFlag() ? 1 : 0));

                setCarryFlag(result > 0xFF);

                setOverflowFlag(((a ^ value) & 0x80) == 0 && ((a ^ result) & 0x80) != 0);

                setNegativeFlag((result & 0x80) != 0);

                setZeroFlag((byte)result == 0);

                a = (byte)result;

                break;
            case 0x8D: // STA $address
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                ushort address = (ushort)(memory[pc + 1] << 8 | memory[pc]);
                memory[address] = a;
                pc += 2;

                break;
            case 0xA9: // LDA #immediate
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                a = memory[pc];
                pc++;

                break;
            case 0xE9: // SBC #immediate
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                value = memory[pc];
                pc++;
                result = (ushort)(a - value - (carryFlag() ? 0 : 1));

                setCarryFlag(result <= 0xFF);

                setOverflowFlag(((a ^ value) & 0x80) != 0 && ((a ^ result) & 0x80) != 0);

                setNegativeFlag((result & 0x80) != 0);

                setZeroFlag((byte)result == 0);

                a = (byte)result;

                break;
            case 0xEA: // NOP
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;

                break;
            default:
                pc++;
                if (debug)
                {
                    System.Console.WriteLine("Unknown instruction: {0:X2}", instuction);
                    printInstructionInfo();
                }
                break;
        }
    }

    public bool carryFlag()
    {
        return (status & 0b00000001) > 0;
    }

    public void setCarryFlag(bool value)
    {
        if (value)
        {
            status |= 0b00000001;
        }
        else
        {
            status &= 0b11111110;
        }
    }

    public bool zeroFlag()
    {
        return (status & 0b00000010) > 0;
    }

    public void setZeroFlag(bool value)
    {
        if (value)
        {
            status |= 0b00000010;
        }
        else
        {
            status &= 0b11111101;
        }
    }
    public bool overflowFlag()
    {
        return (status & 0b01000000) > 0;
    }

    public void setOverflowFlag(bool value)
    {
        if (value)
        {
            status |= 0b01000000;
        }
        else
        {
            status &= 0b10111111;
        }
    }

    public bool negativeFlag()
    {
        return (status & 0b10000000) > 0;
    }

    public void setNegativeFlag(bool value)
    {
        if (value)
        {
            status |= 0b10000000;
        }
        else
        {
            status &= 0b01111111;
        }
    }

    public void printInstructionInfo()
    {
        System.Console.WriteLine("------------------------------");
        System.Console.WriteLine("PC: {0:X4}", pc);
        System.Console.WriteLine("Instruction: {0:X2}", memory[pc]);
    }

    public void printDebugInfo()
    {
        System.Console.WriteLine("SP: {0:X2}", sp);
        System.Console.WriteLine("A: {0:X2}", a);
        System.Console.WriteLine("X: {0:X2}", x);
        System.Console.WriteLine("Y: {0:X2}", y);
        System.Console.WriteLine("Status: {0}", Convert.ToString(status, 2).PadLeft(8, '0'));
        System.Console.WriteLine("0x0200: {0:X2}", memory[0x0200]);
        System.Console.WriteLine("0x0201: {0:X2}", memory[0x0201]);
    }

    public void delayMs()
    {
        Thread.Sleep(1000);
    }
}