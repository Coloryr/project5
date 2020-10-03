#include <Arduino.h>
#include <W25q64\W25Q64.h>
#include <esp_spi_flash.h>

class W25Q64 W25Q64;

void W25Q64::W25Q64_begin(uint32_t frq)
{
    pinMode(SS1, OUTPUT);
    mpSPI = SPIClass(VSPI);
    mpSPI.begin(18, 19, 23, 5);
    // mpSPI.setHwCs(true);
    W25Q64_deselect();
    mSPISettings = SPISettings(frq, MSBFIRST, SPI_MODE0);
}

void W25Q64::W25Q64_end()
{
    W25Q64_powerDown();
    W25Q64_deselect();
    mpSPI.end();
}

void W25Q64::W25Q64_select()
{
    mpSPI.beginTransaction(mSPISettings);
    digitalWrite(SS1, LOW);
}

void W25Q64::W25Q64_deselect()
{
    digitalWrite(SS1, HIGH);
    mpSPI.endTransaction();
}

byte W25Q64::W25Q64_readStatusReg1()
{
    byte rc;
    W25Q64_select();
    mpSPI.transfer(CMD_READ_STATUS_R1);
    rc = mpSPI.transfer(0xFF);
    W25Q64_deselect();
    return rc;
}

byte W25Q64::W25Q64_readStatusReg2()
{
    byte rc;
    W25Q64_select();
    mpSPI.transfer(CMD_READ_STATUS_R2);
    rc = mpSPI.transfer(0xFF);
    W25Q64_deselect();
    return rc;
}

void W25Q64::W25Q64_readUniqieID()
{
    W25Q64_select();
    mpSPI.transfer(CMD_MANUFACURER_ID);
    mpSPI.transfer(0x00);
    mpSPI.transfer(0x00);
    mpSPI.transfer(0x00);
    W25QXX_TYPE |= mpSPI.transfer(0xFF) << 8;
    W25QXX_TYPE |= mpSPI.transfer(0xFF);
    W25Q64_deselect();
}

//等待空闲
boolean W25Q64::W25Q64_IsBusy()
{
    uint8_t r1;
    W25Q64_select();
    mpSPI.transfer(CMD_READ_STATUS_R1);
    r1 = mpSPI.transfer(0xff);
    W25Q64_deselect();
    if (r1 & SR1_BUSY_MASK)
        return true;
    return false;
}

void W25Q64::W25Q64_powerDown()
{
    W25Q64_select();
    mpSPI.transfer(CMD_POWER_DOWN);
    W25Q64_deselect();
}

void W25Q64::W25Q64_WriteEnable()
{
    W25Q64_select();
    mpSPI.transfer(CMD_WRIRE_ENABLE);
    W25Q64_deselect();
}

void W25Q64::W25Q64_WriteDisable()
{
    W25Q64_select();
    mpSPI.transfer(CMD_WRITE_DISABLE);
    W25Q64_deselect();
}

//读取SPI FLASH
//在指定地址开始读取指定长度的数据
//buf:数据存储区
//addr:开始读取的地址(24bit)
//NumByteToRead:要读取的字节数(最大65535)
uint16_t W25Q64::W25Q64_read(uint32_t addr, uint8_t *buf, uint16_t NumByteToRead)
{
    W25Q64_select();
    mpSPI.transfer(CMD_READ_DATA);
    mpSPI.transfer(addr >> 16);         // A23-A16
    mpSPI.transfer((addr >> 8) & 0xFF); // A15-A08
    mpSPI.transfer(addr & 0xFF);        // A07-A00

    uint16_t i;
    for (i = 0; i < NumByteToRead; i++)
    {
        buf[i] = mpSPI.transfer(0xFF);
    }

    W25Q64_deselect();
    return i;
}

//擦除一个扇区
//sect_no:扇区地址 根据实际容量设置
//flgwait:是否等待
//擦除一个扇区的最少时间:150ms
boolean W25Q64::W25Q64_eraseSector(uint16_t sect_no, boolean flgwait)
{
    uint32_t addr = sect_no;
    addr <<= 12;

    W25Q64_WriteEnable();
    W25Q64_select();
    mpSPI.transfer(CMD_SECTOR_ERASE);
    mpSPI.transfer((addr >> 16) & 0xff);
    mpSPI.transfer((addr >> 8) & 0xff);
    mpSPI.transfer(addr & 0xff);
    W25Q64_deselect();

    // 処理待ち
    while (W25Q64_IsBusy() & flgwait)
    {
        delay(10);
    }

    return true;
}

//擦除整个芯片
//flgwait:是否等待
//等待时间超长...
boolean W25Q64::W25Q64_eraseAll(boolean flgwait)
{
    W25Q64_WriteEnable();
    W25Q64_select();
    mpSPI.transfer(CMD_CHIP_ERASE);
    W25Q64_deselect();

    // 処理待ち
    while (W25Q64_IsBusy() & flgwait)
    {
        delay(500);
    }

    W25Q64_deselect();
    return true;
}

//SPI在一页(0~65535)内写入少于256个字节的数据
//在指定地址开始写入最大256字节的数据
//pBuffer:数据存储区
//WriteAddr:开始写入的地址(24bit)
//NumByteToWrite:要写入的字节数(最大256),该数不应该超过该页的剩余字节数!!!
uint16_t W25Q64::W25Q64_pageWrite(uint8_t *pBuffer, uint32_t WriteAddr, uint16_t NumByteToWrite)
{
    uint16_t i;
    W25Q64_WriteEnable();
    while (W25Q64_IsBusy())
    {
        delay(10);
    }

    W25Q64_select();
    mpSPI.transfer(CMD_PAGE_PROGRAM);
    mpSPI.transfer((WriteAddr >> 16) & 0xff);
    mpSPI.transfer((WriteAddr >> 8) & 0xff);
    mpSPI.transfer(WriteAddr & 0xff);

    for (i = 0; i < NumByteToWrite; i++)
    {
        mpSPI.transfer(pBuffer[i]);
    }
    W25Q64_deselect();
    while (W25Q64_IsBusy())
    {
        delay(10);
    }
    return i;
}

void W25Q64::W25Q64_write_nocheck(uint8_t *pBuffer, uint32_t WriteAddr, uint16_t NumByteToWrite)
{
    uint16_t pageremain;
    pageremain = 256 - WriteAddr % 256; //单页剩余的字节数
    if (NumByteToWrite <= pageremain)
        pageremain = NumByteToWrite; //不大于256个字节
    while (1)
    {
        W25Q64_pageWrite(pBuffer, WriteAddr, pageremain);
        if (NumByteToWrite == pageremain)
            break; //写入结束了
        else       //NumByteToWrite>pageremain
        {
            pBuffer += pageremain;
            WriteAddr += pageremain;

            NumByteToWrite -= pageremain; //减去已经写入了的字节数
            if (NumByteToWrite > 256)
                pageremain = 256; //一次可以写入256个字节
            else
                pageremain = NumByteToWrite; //不够256个字节了
        }
    };
}

//写SPI FLASH
//在指定地址开始写入指定长度的数据
//该函数带擦除操作!
//pBuffer:数据存储区
//WriteAddr:开始写入的地址(24bit)
//NumByteToWrite:要写入的字节数(最大65535)
uint16_t W25Q64::W25Q64_Write(uint8_t *pBuffer, uint32_t WriteAddr, uint16_t NumByteToWrite)
{
    uint32_t secpos;
    uint16_t secoff;
    uint16_t secremain;
    uint16_t i;
    uint8_t *W25QXX_BUF;
    W25QXX_BUF = W25QXX_BUFFER;
    secpos = WriteAddr / 4096; //扇区地址
    secoff = WriteAddr % 4096; //在扇区内的偏移
    secremain = 4096 - secoff; //扇区剩余空间大小
    if (NumByteToWrite <= secremain)
        secremain = NumByteToWrite; //不大于4096个字节
    while (1)
    {
        W25Q64_read(secpos * 4096, W25QXX_BUF, 4096); //读出整个扇区的内容
        for (i = 0; i < secremain; i++)               //校验数据
        {
            if (W25QXX_BUF[secoff + i] != 0XFF)
                break; //需要擦除
        }
        if (i < secremain) //需要擦除
        {
            W25Q64_eraseSector(secpos, true); //擦除这个扇区
            for (i = 0; i < secremain; i++)   //复制
            {
                W25QXX_BUF[i + secoff] = pBuffer[i];
            }
            W25Q64_write_nocheck(W25QXX_BUF, secpos * 4096, 4096); //写入整个扇区
        }
        else
            W25Q64_write_nocheck(pBuffer, WriteAddr, secremain); //写已经擦除了的,直接写入扇区剩余区间.
        if (NumByteToWrite == secremain)
            break; //写入结束了
        else       //写入未结束
        {
            secpos++;   //扇区地址增1
            secoff = 0; //偏移位置为0

            pBuffer += secremain;        //指针偏移
            WriteAddr += secremain;      //写地址偏移
            NumByteToWrite -= secremain; //字节数递减
            if (NumByteToWrite > 4096)
                secremain = 4096; //下一个扇区还是写不完
            else
                secremain = NumByteToWrite; //下一个扇区可以写完了
        }
    };
}