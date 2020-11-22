#ifndef DATASAVE_h
#define DATASAVE_h

#include <stdio.h>

extern uint8_t Height;
extern uint8_t Width;
extern uint8_t CanRun;

extern uint8_t **REDData;
extern uint8_t **BULData;

void DataInit();
void FreeShow();
void InitShow();
void SetRun(uint8_t *data, uint8_t start);
void SetShow(uint8_t *data, uint8_t start);

#endif