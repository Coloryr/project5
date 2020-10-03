#ifndef ___W25Q64_h___
#define ___W25Q64_h___

#include <Arduino.h>
#include <SPI.h>

//W25Q64
#define SPI_SLAVE_SEL_PIN 10
#define MAX_BLOCKSIZE 128
#define MAX_SECTORSIZE 2048

#define CMD_WRIRE_ENABLE 0x06
#define CMD_WRITE_DISABLE 0x04
#define CMD_READ_STATUS_R1 0x05
#define CMD_READ_STATUS_R2 0x35
#define CMD_WRITE_STATUS_R 0x01
#define CMD_PAGE_PROGRAM 0x02
#define CMD_QUAD_PAGE_PROGRAM 0x32
#define CMD_BLOCK_ERASE64KB 0xd8
#define CMD_BLOCK_ERASE32KB 0x52
#define CMD_SECTOR_ERASE 0x20
#define CMD_CHIP_ERASE 0xC7
#define CMD_ERASE_SUPPEND 0x75
#define CMD_ERASE_RESUME 0x7A
#define CMD_POWER_DOWN 0xB9
#define CMD_HIGH_PERFORM_MODE 0xA3
#define CMD_CNT_READ_MODE_RST 0xFF
#define CMD_RELEASE_PDOWN_ID 0xAB
#define CMD_MANUFACURER_ID 0x90
#define CMD_READ_UNIQUE_ID 0x4B
#define CMD_JEDEC_ID 0x9f

#define CMD_READ_DATA 0x03
#define CMD_FAST_READ 0x0B
#define CMD_READ_DUAL_OUTPUT 0x3B
#define CMD_READ_DUAL_IO 0xBB
#define CMD_READ_QUAD_OUTPUT 0x6B
#define CMD_READ_QUAD_IO 0xEB
#define CMD_WORD_READ 0xE3

#define SR1_BUSY_MASK 0x01
#define SR1_WEN_MASK 0x02

#define SS1 5
#define SS2 17

class W25Q64
{
private:
    uint8_t W25QXX_BUFFER[4096];
    void W25Q64_end();
    void W25Q64_select();
    void W25Q64_deselect();
    void W25Q64_powerDown();
    void W25Q64_WriteEnable();
    void W25Q64_WriteDisable();
    byte W25Q64_readStatusReg1();
    byte W25Q64_readStatusReg2();

public:
    uint16_t W25QXX_TYPE;
    SPIClass mpSPI;
    SPISettings mSPISettings;
    void W25Q64_readUniqieID();
    boolean W25Q64_IsBusy();
    void W25Q64_begin(uint32_t frq = 1000000);
    void W25Q64_write_nocheck(uint8_t *pBuffer, uint32_t WriteAddr, uint16_t NumByteToWrite);
    uint16_t W25Q64_read(uint32_t addr, uint8_t *buf, uint16_t n);
    boolean W25Q64_eraseSector(uint16_t sect_no, boolean flgwait);
    boolean W25Q64_eraseAll(boolean flgwait);
    uint16_t W25Q64_pageWrite(uint8_t *pBuffer, uint32_t WriteAddr, uint16_t NumByteToWrite);
    uint16_t W25Q64_Write(uint8_t *pBuffer, uint32_t WriteAddr, uint16_t NumByteToWrite);
};

extern class W25Q64 W25Q64;

#endif