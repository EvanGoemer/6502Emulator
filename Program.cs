public class Program
{
    public static void Main()
    {
        Console.WriteLine("This is a library for now. Use unit tests to run code with the emulator.");
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
            case 0x20: // JSR $address
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                ushort addressJSR = (ushort)(memory[pc + 1] << 8 | memory[pc]);
                pc++;
                memory[0x0100 + sp] = (byte)(pc >> 8);
                sp--;
                memory[0x0100 + sp] = (byte)(pc & 0xFF);
                sp--;
                pc = addressJSR;

                break;
            case 0x38: // SEC
                if (debug)
                {
                    printInstructionInfo();
                }

                setCarryFlag(true);
                pc++;

                break;
            case 0x4C: // JMP $address
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                ushort addressJMP = (ushort)(memory[pc + 1] << 8 | memory[pc]);
                pc = addressJMP;

                break;
            case 0x60: // RTS
                if (debug)
                {
                    printInstructionInfo();
                }

                sp++;
                pc = (ushort)(memory[0x0100 + sp + 1] << 8 | memory[0x0100 + sp]);
                pc++;
                sp++;

                break;
            case 0x65: // ADC $address [Zero Page]
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                byte addressZeroPageADC = memory[pc];
                byte valueZeroPageADC = memory[addressZeroPageADC];
                pc++;
                ushort resultZeroPageADC = (ushort)(a + valueZeroPageADC + (carryFlag() ? 1 : 0));

                setCarryFlag(resultZeroPageADC > 0xFF);

                setOverflowFlag(((a ^ valueZeroPageADC) & 0x80) == 0 && ((a ^ resultZeroPageADC) & 0x80) != 0);

                setNegativeFlag((resultZeroPageADC & 0x80) != 0);

                setZeroFlag((byte)resultZeroPageADC == 0);

                a = (byte)resultZeroPageADC;

                break;
            case 0x69: // ADC #immediate
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                byte valueADC = memory[pc];
                pc++;
                ushort resultADC = (ushort)(a + valueADC + (carryFlag() ? 1 : 0));

                setCarryFlag(resultADC > 0xFF);

                setOverflowFlag(((a ^ valueADC) & 0x80) == 0 && ((a ^ resultADC) & 0x80) != 0);

                setNegativeFlag((resultADC & 0x80) != 0);

                setZeroFlag((byte)resultADC == 0);

                a = (byte)resultADC;

                break;
            case 0x85: // STA $address [Zero Page]
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                byte addressZeroPage = memory[pc];
                memory[addressZeroPage] = a;
                pc++;

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
            case 0x9A: // TXS
                if (debug)
                {
                    printInstructionInfo();
                }

                sp = x;
                pc++;

                break;
            case 0xA2: // LDX #immediate
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                x = memory[pc];
                pc++;

                setZeroFlag(x == 0);
                setNegativeFlag((x & 0x80) != 0);

                break;
            case 0xA5: // LDA $address [Zero Page]
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                byte addressZeroPageLDA = memory[pc];
                a = memory[addressZeroPageLDA];
                pc++;

                setZeroFlag(a == 0);
                setNegativeFlag((a & 0x80) != 0);

                break;
            case 0xA9: // LDA #immediate
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                a = memory[pc];
                pc++;

                setZeroFlag(a == 0);
                setNegativeFlag((a & 0x80) != 0);

                break;
            case 0xCA: // DEX
                if (debug)
                {
                    printInstructionInfo();
                }

                x--;
                pc++;

                setZeroFlag(x == 0);
                setNegativeFlag((x & 0x80) != 0);

                break;
            case 0xD0: // BNE #REL
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                sbyte offsetBNE = (sbyte)memory[pc];
                pc++;
                if (!zeroFlag())
                {
                    pc += (ushort)offsetBNE;
                }

                break;
            case 0xE5: // SBC $address [Zero Page]
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                byte addressZeroPageSBC = memory[pc];
                byte valueZeroPageSBC = memory[addressZeroPageSBC];
                pc++;
                ushort resultZeroPageSBC = (ushort)(a - valueZeroPageSBC - (carryFlag() ? 0 : 1));

                setCarryFlag(resultZeroPageSBC <= 0xFF);

                setOverflowFlag(((a ^ valueZeroPageSBC) & 0x80) != 0 && ((a ^ resultZeroPageSBC) & 0x80) != 0);

                setNegativeFlag((resultZeroPageSBC & 0x80) != 0);

                setZeroFlag((byte)resultZeroPageSBC == 0);

                a = (byte)resultZeroPageSBC;

                break;
            case 0xE9: // SBC #immediate
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                byte valueSBC = memory[pc];
                pc++;
                ushort resultSBC = (ushort)(a - valueSBC - (carryFlag() ? 0 : 1));

                setCarryFlag(resultSBC <= 0xFF);

                setOverflowFlag(((a ^ valueSBC) & 0x80) != 0 && ((a ^ resultSBC) & 0x80) != 0);

                setNegativeFlag((resultSBC & 0x80) != 0);

                setZeroFlag((byte)resultSBC == 0);

                a = (byte)resultSBC;

                break;
            case 0xEA: // NOP
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                break;
            case 0XF0: // BEQ #REL
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                sbyte offsetBEQ = (sbyte)memory[pc];
                pc++;
                if (zeroFlag())
                {
                    pc += (ushort)offsetBEQ;
                }
                break;
            default:
                if (debug)
                {
                    printInstructionInfo();
                    System.Console.WriteLine("Unknown instruction: {0:X2}", instuction);
                }
                isRunning = false;
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