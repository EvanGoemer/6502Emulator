﻿public class Program
{
    public static void Main()
    {
        byte[] memory = new byte[64 * 1024];
        memory[0xFFFC] = 0x00;
        memory[0xFFFD] = 0x00;

        // add 2+2 and put the result in memory at 0x200
        memory[0x0000] = 0xA9; // LDA #2
        memory[0x0001] = 0x02;
        memory[0x0002] = 0x69; // ADC #2
        memory[0x0003] = 0x02;
        memory[0x0004] = 0x8D; // STA $0200
        memory[0x0005] = 0x00;
        memory[0x0006] = 0x02;
        memory[0x0007] = 0x02; // HLT
        Emulator6502 emulator = new Emulator6502(memory);
        while (emulator.isRunning)
        {
            emulator.executeNextInstruction();
            emulator.printDebugInfo();
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
        status = 0b00110110;
    }

    public void executeNextInstruction()
    {
        byte instuction = memory[pc];
        switch (instuction)
        {
            case 0x02: // HLT
                isRunning = false;

                if(debug)
                {
                    printInstructionInfo();
                }
                break;
            case 0x69: // ADC #immediate
                if (debug)
                {
                    printInstructionInfo();
                }

                pc++;
                byte value = memory[pc];
                pc++;
                ushort result = (ushort)(a + value);
                if(carryFlag())
                {
                    result++;
                }

                if (result > 0xFF)
                {
                    setCarryFlag(true);
                }
                else
                {
                    setCarryFlag(false);
                }

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
        System.Console.WriteLine("Status: {0:X2}", status);
        System.Console.WriteLine("0x0200: {0:X2}", memory[0x0200]);
    }

    public void delayMs()
    {
        Thread.Sleep(1000);
    }
}