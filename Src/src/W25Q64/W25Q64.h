#ifndef ___W25Q64_h___
#define ___W25Q64_h___

#include <Arduino.h>
#include <SPI.h>

//W25Q64
#define SPI_SLAVE_SEL_PIN 10 // チップセレクトピン番号
#define MAX_BLOCKSIZE 128    // ブロック総数
#define MAX_SECTORSIZE 2048  // 総セクタ数

#define CMD_WRIRE_ENABLE 0x06
#define CMD_WRITE_DISABLE 0x04
#define CMD_READ_STATUS_R1 0x05
#define CMD_READ_STATUS_R2 0x35
#define CMD_WRITE_STATUS_R 0x01 // 未実装
#define CMD_PAGE_PROGRAM 0x02
#define CMD_QUAD_PAGE_PROGRAM 0x32 // 未実装
#define CMD_BLOCK_ERASE64KB 0xd8
#define CMD_BLOCK_ERASE32KB 0x52
#define CMD_SECTOR_ERASE 0x20
#define CMD_CHIP_ERASE 0xC7
#define CMD_ERASE_SUPPEND 0x75 // 未実装
#define CMD_ERASE_RESUME 0x7A  // 未実装
#define CMD_POWER_DOWN 0xB9
#define CMD_HIGH_PERFORM_MODE 0xA3 // 未実装
#define CMD_CNT_READ_MODE_RST 0xFF // 未実装
#define CMD_RELEASE_PDOWN_ID 0xAB  // 未実装
#define CMD_MANUFACURER_ID 0x90
#define CMD_READ_UNIQUE_ID 0x4B
#define CMD_JEDEC_ID 0x9f

#define CMD_READ_DATA 0x03
#define CMD_FAST_READ 0x0B
#define CMD_READ_DUAL_OUTPUT 0x3B // 未実装
#define CMD_READ_DUAL_IO 0xBB     // 未実装
#define CMD_READ_QUAD_OUTPUT 0x6B // 未実装
#define CMD_READ_QUAD_IO 0xEB     // 未実装
#define CMD_WORD_READ 0xE3        // 未実装

#define SR1_BUSY_MASK 0x01
#define SR1_WEN_MASK 0x02

class W25Q64
{
private:
    void W25Q64_begin(uint8_t cs, uint32_t frq = 8000000); // フラッシュメモリ W25Q64の利用開始
    void W25Q64_end();                                     // フラッシュメモリ W25Q64の利用終了
    void W25Q64_select();                                  // チップセレクト フラッシュメモリ操作を選択にする
    void W25Q64_deselect();                                // チップディセレクト フラッシュメモリ操作を有非選択にする
    byte W25Q64_readStatusReg1();                          // ステータスレジスタ1の値取得
    byte W25Q64_readStatusReg2();                          // ステータスレジスタ2の値取得
    void W25Q64_readManufacturer(byte *d);                 // JEDEC ID(Manufacture, Memory Type,Capacity)の取得
    void W25Q64_readUniqieID(byte *d);                     // Unique IDの取得
    boolean W25Q64_IsBusy();                               // 書込み等の処理中チェック
    void W25Q64_powerDown();                               // パワーダウン指定
    void W25Q64_WriteEnable();                             // 書込み許可設定
    void W25Q64_WriteDisable();                            // 書込み禁止設定
    void W25Q64_setSPIPort(SPIClass &rSPI);                // SPIポートの設定
public:
    uint16_t W25Q64_read(uint32_t addr, uint8_t *buf, uint16_t n);                        // データの読み込み
    uint16_t W25Q64_fastread(uint32_t addr, uint8_t *buf, uint16_t n);                    // 高速データの読み込み
    boolean W25Q64_eraseSector(uint16_t sect_no, boolean flgwait);                        // セクタ単位消去
    boolean W25Q64_erase64Block(uint16_t blk_no, boolean flgwait);                        // 64KBブロック単位消去
    boolean W25Q64_erase32Block(uint16_t blk_no, boolean flgwait);                        // 32KBブロック単位消去
    boolean W25Q64_eraseAll(boolean flgwait);                                             // 全領域の消去
    uint16_t W25Q64_pageWrite(uint16_t sect_no, uint16_t inaddr, byte *data, uint16_t n); // データの書き込み
}

#endif