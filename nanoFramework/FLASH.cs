using nanoFramework.Hardware.Esp32;
using System;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace NFApp1
{
    class Command
    {
        public const byte SPI_SLAVE_SEL_PIN = 10;
        public const int MAX_BLOCKSIZE = 128;
        public const int MAX_SECTORSIZE = 2048;

        public const byte CMD_WRIRE_ENABLE = 0x06;
        public const byte CMD_WRITE_DISABLE = 0x04;
        public const byte CMD_READ_STATUS_R1 = 0x05;
        public const byte CMD_READ_STATUS_R2 = 0x35;
        public const byte CMD_WRITE_STATUS_R = 0x01;
        public const byte CMD_PAGE_PROGRAM = 0x02;
        public const byte CMD_QUAD_PAGE_PROGRAM = 0x32;
        public const byte CMD_BLOCK_ERASE64KB = 0xd8;
        public const byte CMD_BLOCK_ERASE32KB = 0x52;
        public const byte CMD_SECTOR_ERASE = 0x20;
        public const byte CMD_CHIP_ERASE = 0xC7;
        public const byte CMD_ERASE_SUPPEND = 0x75;
        public const byte CMD_ERASE_RESUME = 0x7A;
        public const byte CMD_POWER_DOWN = 0xB9;
        public const byte CMD_HIGH_PERFORM_MODE = 0xA3;
        public const byte CMD_CNT_READ_MODE_RST = 0xFF;
        public const byte CMD_RELEASE_PDOWN_ID = 0xAB;
        public const byte CMD_MANUFACURER_ID = 0x90;
        public const byte CMD_READ_UNIQUE_ID = 0x4B;
        public const byte CMD_JEDEC_ID = 0x9f;

        public const byte CMD_READ_DATA = 0x03;
        public const byte CMD_FAST_READ = 0x0B;
        public const byte CMD_READ_DUAL_OUTPUT = 0x3B;
        public const byte CMD_READ_DUAL_IO = 0xBB;
        public const byte CMD_READ_QUAD_OUTPUT = 0x6B;
        public const byte CMD_READ_QUAD_IO = 0xEB;
        public const byte CMD_WORD_READ = 0xE3;

        public const byte SR1_BUSY_MASK = 0x01;
        public const byte SR1_WEN_MASK = 0x02;
    }
    class FLASH
    {
        private SpiDevice spi;
        private GpioPin cs1;
        public FLASH()
        {
            var connectionSettings = new SpiConnectionSettings(12)
            {
                DataBitLength = 8,
                ClockFrequency = 10000000
            };
            spi = SpiDevice.FromId("SPI2", connectionSettings);

            cs1 = GpioController.GetDefault().OpenPin(5);
            cs1.SetDriveMode(GpioPinDriveMode.Output);
            cs1.Write(GpioPinValue.High);
        }
        public int GetID()
        {
            var data = new byte[4] { Command.CMD_MANUFACURER_ID, 0x00, 0x00, 0x00 };
            var res = new byte[2];
            cs1.Write(GpioPinValue.Low);
            spi.Write(data);
            spi.TransferFullDuplex(res, res);
            cs1.Write(GpioPinValue.High);
            byte temp = res[1];
            res[1] = res[0];
            res[0] = temp;
            ushort test = BitConverter.ToUInt16(res, 0);
            return test;
        }
        public byte ReadStatusReg1()
        {
            var data = new byte[2] { Command.CMD_READ_STATUS_R1, 0x00 };
            var res = new byte[2];
            cs1.Write(GpioPinValue.Low);
            spi.TransferFullDuplex(data, res);
            cs1.Write(GpioPinValue.High);
            return res[1];
        }
        public byte ReadStatusReg2()
        {
            var data = new byte[2] { Command.CMD_READ_STATUS_R2, 0x00 };
            var res = new byte[2];
            cs1.Write(GpioPinValue.Low);
            spi.TransferFullDuplex(data, res);
            cs1.Write(GpioPinValue.High);
            return res[1];
        }
        public bool IsBusy()
        {
            var data = new byte[2] { Command.CMD_READ_STATUS_R1, 0x00 };
            var res = new byte[2];
            cs1.Write(GpioPinValue.Low);
            spi.TransferFullDuplex(data, res);
            cs1.Write(GpioPinValue.High);
            if ((res[1] & Command.SR1_BUSY_MASK) == 0x01)
                return true;
            return false;
        }
        public void PowerDown()
        {
            cs1.Write(GpioPinValue.Low);
            spi.Write(new byte[] { Command.CMD_POWER_DOWN });
            cs1.Write(GpioPinValue.High);
        }
        public void WriteEnable()
        {
            cs1.Write(GpioPinValue.Low);
            spi.Write(new byte[] { Command.CMD_WRIRE_ENABLE });
            cs1.Write(GpioPinValue.High);
        }
        public void WriteDisable()
        {
            cs1.Write(GpioPinValue.Low);
            spi.Write(new byte[] { Command.CMD_WRITE_DISABLE });
            cs1.Write(GpioPinValue.High);
        }
        public byte[] Read(long addr, int NumByteToRead)
        {
            var temp = new byte[] { Command.CMD_READ_DATA, (byte)(addr >> 16), (byte)(addr >> 8), (byte)addr };
            var data = new byte[NumByteToRead];
            cs1.Write(GpioPinValue.Low);
            spi.Write(temp);
            spi.TransferFullDuplex(data, data);
            cs1.Write(GpioPinValue.High);
            return data;
        }
        public void EraseSector(long addr, bool wait = true)
        {
            addr *= 4096;

            while (IsBusy())
            {
                Thread.Sleep(10);
            }
            WriteEnable();
            var temp = new byte[] { Command.CMD_SECTOR_ERASE, (byte)(addr >> 16), (byte)(addr >> 8), (byte)addr };
            cs1.Write(GpioPinValue.Low);
            spi.Write(temp);
            cs1.Write(GpioPinValue.High);
            while (IsBusy() && wait)
            {
                Thread.Sleep(10);
            }
        }
        public void EraseAll(bool wait = true)
        {
            WriteEnable();
            while (IsBusy())
            {
                Thread.Sleep(10);
            }
            var temp = new byte[] { Command.CMD_CHIP_ERASE };
            cs1.Write(GpioPinValue.Low);
            spi.Write(temp);
            cs1.Write(GpioPinValue.High);
            while (IsBusy() && wait)
            {
                Thread.Sleep(10);
            }
        }
        public void PageWrite(byte[] data, long addr)
        {
            WriteEnable();
            var temp = new byte[4] { Command.CMD_PAGE_PROGRAM, (byte)(addr >> 16), (byte)(addr >> 8), (byte)addr };
            cs1.Write(GpioPinValue.Low);
            spi.Write(temp);
            spi.Write(data);
            cs1.Write(GpioPinValue.High);
            WriteDisable();
        }
        public void WriteNocheck(byte[] data, long addr)
        {
            int pageremain;
            int NumByteToWrite = data.Length;
            int now = 0;
            pageremain = (int)(256 - addr % 256); //单页剩余的字节数
            if (NumByteToWrite <= pageremain)
            {
                PageWrite(data, addr);
                return;
            }
            while (true)
            {
                var temp = new byte[pageremain];
                Array.Copy(data, now, temp, 0, pageremain);
                PageWrite(temp, addr);
                if (NumByteToWrite == pageremain)
                    break; //写入结束了
                else       //NumByteToWrite>pageremain
                {
                    now += pageremain;
                    addr += pageremain;
                    NumByteToWrite -= pageremain; //减去已经写入了的字节数
                    if (NumByteToWrite > 256)
                        pageremain = 256; //一次可以写入256个字节
                    else
                        pageremain = NumByteToWrite; //不够256个字节了
                }
            };
        }
        public void Write(byte[] data, long WriteAddr)
        {
            long secpos;
            int NumByteToWrite = data.Length;
            int secoff;
            int secremain;
            int i;
            int now = 0;
            byte[] W25QXX_BUF;
            secpos = WriteAddr / 4096; //扇区地址
            secoff = (int)(WriteAddr % 4096); //在扇区内的偏移
            secremain = 4096 - secoff; //扇区剩余空间大小
            if (NumByteToWrite <= secremain)
                secremain = NumByteToWrite; //不大于4096个字节
            while (true)
            {
                W25QXX_BUF = Read(secpos * 4096, 4096); //读出整个扇区的内容
                for (i = 0; i < secremain; i++)               //校验数据
                {
                    if (W25QXX_BUF[secoff + i] != 0XFF)
                        break; //需要擦除
                }
                if (i < secremain) //需要擦除
                {
                    EraseSector(secpos, true); //擦除这个扇区
                    Array.Copy(data, now, W25QXX_BUF, 0, secremain);
                    WriteNocheck(W25QXX_BUF, secpos * 4096); //写入整个扇区
                }
                else
                {
                    W25QXX_BUF = new byte[secremain];
                    Array.Copy(data, now, W25QXX_BUF, 0, secremain);
                    WriteNocheck(W25QXX_BUF, WriteAddr); //写已经擦除了的,直接写入扇区剩余区间.
                }
                if (NumByteToWrite == secremain)
                    break; //写入结束了
                else       //写入未结束
                {
                    secpos++;   //扇区地址增1
                    secoff = 0; //偏移位置为0

                    now += secremain;        //指针偏移
                    WriteAddr += secremain;      //写地址偏移
                    NumByteToWrite -= secremain; //字节数递减
                    if (NumByteToWrite > 4096)
                        secremain = 4096; //下一个扇区还是写不完
                    else
                        secremain = NumByteToWrite; //下一个扇区可以写完了
                }
            };
        }
    }
}
