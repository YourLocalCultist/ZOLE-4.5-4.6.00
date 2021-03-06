﻿using System;
using System.Collections.Generic;

using System.Text;

namespace ZOLE_4
{
    public class MapSaver
    {
        public GBHL.GBFile gb;
        public MapSaver(GBHL.GBFile g)
        {
            gb = g;
        }

        public void expandROM()
        {
            byte[] newBuffer = new byte[0x400000];
            Array.Copy(gb.Buffer, newBuffer, 0x100000);
            gb.Buffer = newBuffer;
            //Fix the header
            gb.WriteByte(0x148, 7); //Write new ROM size
            byte b = gb.ReadByte(0x14D); //0x39 -> 0x38
            b -= 2;
            gb.WriteByte(0x14D, b); //Fix checksum

            writeASM();
        }

        public void expand2MROM()
        {
            byte[] newBuffer = new byte[0x400000];
            Array.Copy(gb.Buffer, newBuffer, 0x200000);
            gb.Buffer = newBuffer;
            //Fix the header
            gb.WriteByte(0x148, 7); //Write new ROM size
            byte b = gb.ReadByte(0x14D); //0x39 -> 0x38
            b--;
            gb.WriteByte(0x14D, b); //Fix checksum
            writeASM();
        }

        public void expandSeasonsROM()
        {
            byte[] newBuffer = new byte[0x200000];
            Array.Copy(gb.Buffer, newBuffer, 0x100000);
            gb.Buffer = newBuffer;
            //Fix the header
            gb.WriteByte(0x148, 6); //Write new ROM size
            byte b = gb.ReadByte(0x14D); //0x39 -> 0x38
            b -= 1;
            gb.WriteByte(0x14D, b); //Fix checksum
            WriteSeasonsASM();
            PatchSeasonsTables();
        }

        public void writeASM()
        {
            //Write a new asm script
            gb.WriteBytes(0x3A28, new byte[] { 0x3E, 0x54, 0xCD, 0xF8, 0x3E });
            byte[] script = new byte[] { 0xD5, 0xE5, 0xC5, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0x21, 0x00, 0x40, 0xCD, 0x30, 0x46, 0xFE, 0x06, 0x20, 0x02, 0x3E, 0x04, 0xFE, 0x07, 0x20, 0x02, 0x3E, 0x05, 0xDF, 0x2A, 0x66, 0x6F, 0xFA, 0x2F, 0xCC, 0x06, 0x00, 0x4F, 0x09, 0x7E, 0xC1, 0xE1, 0xD1, 0x5F, 0xAF, 0x7C, 0xE6, 0x80, 0xC8, 0x7C, 0xD6, 0x40, 0x67, 0xAF, 0xC9 };
            gb.WriteBytes(0x3EF8, script);
            //Dungeon script. Right now the game doesn't support fully uncompressed dungeon data
            script = new byte[] { 0x11, 0x00, 0xCF, 0x2A, 0x12, 0x1C, 0x7B, 0xFE, 0xFF, 0xC8, 0xC3, 0x4C, 0x39 };
            gb.WriteBytes(0x3949, script);
            //Custom VRAM loading
            //script = new byte[] { 0x7E, 0xFE, 0xFF, 0x28, 0x06, 0x2A, 0x66, 0x6F, 0xC3, 0x3D, 0x06, 0x23, 0x2A, 0x26, 0x50, 0x47, 0x80, 0x80, 0x3E, 0x40, 0xE0, 0x97, 0x6F, 0x30, 0x01, 0x24, 0xEA, 0x22, 0x22, 0x2A, 0x47, 0x2A, 0x66, 0x6F, 0x78, 0xEA, 0x22, 0x22, 0xF6, 0x01, 0xE0, 0x4F, 0x11, 0x00, 0x88, 0x01, 0x00, 0x11, 0x2A, 0x12, 0x13, 0x0D, 0x79, 0xFE, 0xFF, 0x20, 0xF7, 0x05, 0x20, 0xF4, 0xC9 };
            //gb.WriteBytes(0x3F2D, script);
            //gb.WriteBytes(0x63A, new byte[] { 0xC3, 0x2D, 0x3F });
            //Allowing map swapping
            script = new byte[] { 0xFA, 0x2D, 0xCC, 0xFE, 0x02, 0xD0, 0x47, 0xFE, 0x00, 0x20, 0x0A, 0xFA, 0x24, 0xCD, 0xFE, 0x01, 0x3E, 0x02, 0xC8, 0x78, 0xC9, 0xFA, 0x24, 0xCD, 0xFE, 0x03, 0xC8, 0x78, 0xC9 };
            gb.WriteBytes(0x150630, script);
            //100% custom vram loading
            /*gb.WriteBytes(0x180000, new byte[] { 0xF0, 0x8D, 0xCD, 0x23, 0x40, 0x00, 0x2A, 0xFE, 0x00, 0x20, 0x07, 0xFA, 0x21, 0xCD, 0xCD, 0x26, 0x06, 0xC9, 0xF5, 0x2A, 0x66, 0x6F, 0x01, 0x00, 0x10, 0x11, 0x00, 0x88, 0x3E, 0x01, 0xE0, 0x4F, 0xC3, 0x7A, 0x3F, 0x21, 0x2C, 0x40, 0x06, 0x00, 0x4F, 0x09, 0x09, 0x09, 0xC9 });
            gb.WriteBytes(0x3F7A, new byte[] { 0xF3, 0xF1, 0xEA, 0x00, 0x20, 0x2A, 0x12, 0x13, 0x0B, 0xB1, 0x20, 0xF9, 0xB0, 0x20, 0xF6, 0xFA, 0x22, 0xCD, 0xCD, 0x0B, 0x05, 0x37, 0xFB, 0xC9 });
            gb.WriteBytes(0x3F92, new byte[] { 0x3E, 0x60, 0xEA, 0x00, 0x20, 0xC3, 0x00, 0x40 });
            gb.WriteBytes(0x3799, new byte[] { 0xCD, 0x92, 0x3F, 0x38, 0x0A, 0x00 });
            gb.WriteBytes(0x13EE2, new byte[] { 0xCD, 0xD6, 0x6D, 0x21, 0xD4, 0x52, 0xDF, 0x2A, 0x66, 0x6F, 0xFA, 0x30, 0xCC, 0xD7, 0x7E, 0xC9 });
            gb.WriteBytes(0x37DB, new byte[] { 0xCD, 0x9A, 0x3F, 0xD8, 0x00 });
            gb.WriteBytes(0x3F9A, new byte[] { 0xF0, 0x8D, 0xF5, 0x3E, 0x04, 0xEA, 0x22, 0x22, 0xCD, 0xE2, 0x7E, 0xE0, 0x8D, 0x3E, 0x60, 0xEA, 0x22, 0x22, 0xFA, 0x28, 0xCD, 0xFE, 0xFF, 0x28, 0x0C, 0xCD, 0x23, 0x40, 0xFE, 0x00, 0x20, 0x05, 0xF1, 0xE0, 0x8D, 0x18, 0x06, 0xCD, 0x00, 0x40, 0xF1, 0xE0, 0x8D, 0x3E, 0x01, 0xEA, 0x22, 0x22, 0xC9 });*/
            
            gb.WriteBytes(0x3F96, new byte[] { 0xF0, 0x97, 0xF5, 0x3E, 0x04, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0xCD, 0xE2, 0x7E, 0xE6, 0x7F, 0xE0, 0x8D, 0x3E, 0x60, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0xC3, 0x35, 0x41 });

            // Lin's old VRAM patch
            //gb.WriteBytes(0x180135, new byte[] { 0xF0, 0x8D, 0x4F, 0x06, 0x00, 0x21, 0x00, 0x40, 0x09, 0x09, 0x09, 0x2A, 0xF5, 0x2A, 0x66, 0x6F, 0xF1, 0xC3, 0x78, 0x3F });
            // New VRAM patch (works on real hardware)
            // This patch makes the patch at 3f78 irrelevant
            gb.WriteBytes(0x180135, new byte[] { 0xF0, 0x8D, 0x4F, 0x06, 0x00, 0x21, 0x00, 0x40, 0x09, 0x09, 0x09, 0x2A, 0x4F, 0x2A, 0x66, 0x6F, 0x11, 0x01, 0x88, 0xF0, 0x40, 0xE6, 0x80, 0x20, 0x16, 0x06, 0x7F, 0xC5, 0xE5, 0xCD, 0x8A, 0x05, 0xE1, 0x01, 0x00, 0x08, 0x09, 0xC1, 0x11, 0x01, 0x90, 0xCD, 0x8A, 0x05, 0xC3, 0x8D, 0x41, 0x06, 0x54, 0xC5, 0xE5, 0xCD, 0x8A, 0x05, 0xCD, 0x90, 0x41, 0xE1, 0x01, 0x50, 0x05, 0x09, 0xC1, 0x11, 0x51, 0x8D, 0xC5, 0xE5, 0xCD, 0x8A, 0x05, 0xCD, 0x90, 0x41, 0xE1, 0x01, 0x50, 0x05, 0x09, 0xC1, 0x06, 0x55, 0x11, 0xA1, 0x92, 0xCD, 0x8A, 0x05, 0xC3, 0x98, 0x00, 0x21, 0x9D, 0xC4, 0x36, 0xFF, 0x76, 0x00, 0xCB, 0x7E, 0x20, 0xF5, 0xC9 });


            gb.WriteBytes(0x3F78, new byte[] { 0x01, 0x00, 0x10, 0x11, 0x00, 0x88, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0x3E, 0x01, 0xE0, 0x4F, 0x2A, 0x12, 0x13, 0x0B, 0x78, 0xB1, 0x20, 0xF8, 0xF1, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0xC9 });
            gb.WriteBytes(0x3799, new byte[] { 0xCD, 0x96, 0x3F, 0xFA, 0x22, 0xCD, 0xCD, 0x0B, 0x05, 0x18, 0x04 });
            gb.WriteBytes(0x37DB, new byte[] { 0x3E, 0x04, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0xF0, 0x8D, 0xF5, 0xCD, 0xE2, 0x7E, 0xE6, 0x7F, 0xE0, 0x8D, 0xCD, 0x96, 0x3F, 0x3E, 0x01, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0xF1, 0xE0, 0x8D, 0xFE, 0x00, 0xC9 });
            gb.WriteBytes(0x13EE2, new byte[] { 0xCD, 0xD6, 0x6D, 0x21, 0xD4, 0x52, 0xDF, 0x2A, 0x66, 0x6F, 0xFA, 0x30, 0xCC, 0xD7, 0x7E, 0xC9 });
            //All above lines are OK
            gb.WriteBytes(0x372B, new byte[] { 0xCD, 0x3B, 0x37, 0x21, 0x63, 0x6E, 0x1E, 0x04, 0xC3, 0x8A, 0x00 });
            gb.WriteBytes(0x373B, new byte[] { 0x3E, 0x80, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0x21, 0x00, 0x40, 0xFA, 0x23, 0xCD, 0x4F, 0x06, 0x00, 0x09, 0x09, 0x09, 0x2A, 0xF5, 0x2A, 0x66, 0x6F, 0xF1, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0x01, 0x00, 0x08, 0x11, 0x00, 0xD0, 0x3E, 0x03, 0xE0, 0x70, 0x2A, 0x12, 0x13, 0x0B, 0x79, 0xB0, 0x20, 0xF8, 0xC9 });
            //gb.WriteBytes(0x3FAD, new byte[] { 0xCD, 0x3B, 0x37, 0xC9 }); //OK
            gb.WriteBytes(0x38A8, new byte[] { 0xCD, 0x3B, 0x37, 0xFA, 0x23, 0xCD, 0xEA, 0x2A, 0xCD, 0xCD, 0x12, 0x37, 0x3C, 0x3D });
            //gb.WriteBytes(0x3FF3, new byte[] { 0xEA, 0x22, 0x22, 0xAF, 0xE0, 0x4F, 0xC9 });
        }

        public void WriteSeasonsASM()
        {
            gb.WriteBytes(0x39DF, new byte[] { 0xCD, 0xC8, 0x3E, 0x7B, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0x06, 0xB0, 0x11, 0x00, 0xCF, 0xCD, 0x64, 0x07, 0x12, 0x1C, 0x05, 0x78, 0xFE, 0x00, 0x20, 0xF5, 0x21, 0x00, 0xCF, 0xAF, 0x5F, 0xC9 });
            gb.WriteBytes(0x3EC8, new byte[] { 0x3E, 0x42, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0xFA, 0x24, 0xCD, 0xCD, 0x15, 0x5C, 0x3E, 0x42, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0x21, 0x00, 0x40, 0xC5, 0x7A, 0x87, 0x87, 0x47, 0x0E, 0x00, 0x09, 0xC1, 0xFA, 0x4B, 0xCC, 0xDF, 0xDF, 0x2A, 0x2A, 0x5F, 0x2A, 0x66, 0x6F, 0xC9 }); //0xC5, 0xD5, 0xE5, 0x3E, 0x5C, 0xE0, 0x97, 0xEA, 0x22, 0x22, 0xFA, 0x24, 0xCD, 0x21, 0x00, 0x40, 0xDF, 0x2A, 0x66, 0x6F, 0xFA, 0x4B, 0xCC, 0x06, 0x00, 0x4F, 0x09, 0x2A, 0xE1, 0xD1, 0xC1, 0x5F, 0x7C, 0xE6, 0x80, 0xC8, 0x7C, 0xD6, 0x40, 0x67, 0xAF, 0xC9 });
            gb.WriteBytes(0x395B, new byte[] { 0x11, 0x00, 0xCE, 0x2A, 0x12, 0x13, 0x7B, 0xFE, 0xB0, 0x20, 0xF8, 0x11, 0x00, 0xCF, 0x21, 0x00, 0xCE, 0x01, 0x08, 0x0A, 0xC5, 0x2A, 0x12, 0x1C, 0x05, 0x20, 0xFA, 0x7B, 0xC6, 0x06, 0x5F, 0xC1, 0x0D, 0x20, 0xF1, 0xC9 });
            //gb.WriteBytes(0x3EF2, new byte[] { 0xFA, 0x24, 0xCD, 0xFE, 0x04, 0x28, 0x06, 0x30, 0x08, 0xC6, 0x07, 0x18, 0x07, 0x3E, 0x01, 0x18, 0x03, 0xFA, 0x49, 0xCC, 0xC9 });
            //Allow uncompressed dungeon data
            gb.WriteBytes(0x3901, new byte[] { 0x11, 0x00, 0xCF, 0x2A, 0x12, 0x1C, 0x7B, 0xFE, 0xFF, 0xC8, 0xC3, 0x04, 0x39 });
            //gb.WriteBytes(0x38A3, new byte[] { 0xCD, 0x07, 0x3F });
            //gb.WriteBytes(0x3F07, new byte[] { 0xFA, 0x24, 0xCD, 0xFE, 0x04, 0x30, 0x03, 0xC9, 0xFE, 0x05, 0x20, 0x03, 0x3E, 0x01, 0xC9, 0xC9 });
        }

        public void PatchSeasonsTables()
        {
            byte[] animalMaps = new byte[] { 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x69, 0x6A, 0x6B, 0x6C, 0x79, 0x7A, 0x7B };
            gb.WriteBytes(0x109C00, animalMaps);
            gb.WriteBytes(new byte[] { 0x21, 0x00, 0x5C, 0x57, 0xFA, 0x49, 0xCC, 0xFE, 0x00, 0xC0, 0xFA, 0x4B, 0xCC, 0x47, 0x7D, 0xFE, 0x15, 0xC8, 0x2A, 0xB8, 0x20, 0xF8, 0xFA, 0xAF, 0xC6, 0xEA, 0x24, 0xCD, 0xEA, 0x4E, 0xCC, 0x57, 0xC6, 0x0A, 0xEA, 0x10, 0xC6, 0xC9 });
            //gb.WriteBytes(0x7E44, new byte[] { 0xFA, 0xAF, 0xC6, 0x00, 0x00, 0x00, 0x00 });
        }

        public void decompressGroup(int group, MapLoader mapLoader, Program.GameTypes game)
        {
            if (gb.Buffer.Length == 0x100000)
            {
                throw new Exception("ROM has not been expanded.");
            }

            if (group < 4)
            {
                int address = 0x104000 + (group * 2) * 0x4000; //Bank 0x41

                for (int i = 0; i < 256; i++)
                {
                    /*int rg = group;
                    if (group == 1)
                        rg = 2;
                    else if (group == 2)
                        rg = 1;*/
                    mapLoader.loadMap(i, group, 0, false, game);
                    MapLoader.Room r = mapLoader.room;

                    gb.BufferLocation = address;
                    gb.WriteBytes(r.decompressed);
                    int count = gb.BufferLocation - address;
                    address = gb.BufferLocation;

                    //Write the new bank indexes
                    gb.BufferLocation = 0x54 * 0x4000 + group * 2;
                    gb.WriteBytes(gb.Get2BytePointer(group * 0x100 + 0x20));
                    gb.BufferLocation = 0x54 * 0x4000 + group * 0x100 + 0x20 + i;
                    gb.WriteByte((byte)((address - count) / 0x4000));
                    //Write the map-specific header
                    gb.BufferLocation = 0x40 * 0x4000 + 0x200 + (0x200 * group) + (i * 2);
                    byte[] b = gb.Get2BytePointer(address - count);
                    gb.WriteBytes(new byte[] { b[0], (byte)(b[1] - 0x40) });
                    //gb.WriteBytes(new byte[] { (byte)(count + (address % 0x4000)), (byte)((count + (address % 0x4000)) >> 8) });
                }
            }
            else
            {
                int address = 0x104000 + (group * 3) * 0x4000; //Bank 0x41
                for (int i = 0; i < 256; i++)
                {
                    mapLoader.loadMap(i, group, 0, false, game);
                    MapLoader.Room r = mapLoader.room;

                    gb.BufferLocation = address;
                    gb.WriteBytes(r.decompressed);
                    int count = gb.BufferLocation - address;
                    address = gb.BufferLocation;

                    //Write the new bank indexes
                    gb.BufferLocation = 0x54 * 0x4000 + group * 2;
                    gb.WriteBytes(gb.Get2BytePointer(group * 0x100 + 0x20));
                    gb.BufferLocation = 0x54 * 0x4000 + group * 0x100 + 0x20 + i;
                    gb.WriteByte((byte)((address - count) / 0x4000));

                    gb.BufferLocation = 0x40 * 0x4000 + 0x200 + (0x200 * group) + (i * 2);
                    byte[] p = gb.Get2BytePointer(address - count);
                    gb.WriteBytes(new byte[] { p[0], (byte)(p[1] + 2) }); //TEMPORARY!
                }
            }

            //Write the new region header
            int indexBase = 0x10000 + (game == Program.GameTypes.Ages ? 0x0F6C : 0x0C4C);
            byte a = (byte)(group * 8);
            gb.BufferLocation = indexBase + a;
            gb.WriteByte((byte)(group < 4 ? 1 : 0)); //The map type
            gb.WriteByte(0x40); //The header pointers will always be in bank 0x40
            if (group < 4)
                gb.WriteBytes(new byte[] { 0, (byte)(0x42 + (0x2 * group)) }); // The pointer. 0x200 bytes per group
            else
                gb.WriteBytes(new byte[] { 0, (byte)(0x32 + (0x2 * group)) }); //The relative minor pointer. Only 41 because the loading procedure address 0x1000
            if (group < 4)
                gb.WriteByte((byte)(0x41 + (group * 2)));
            else
                gb.WriteByte((byte)(0x46 + ((group - 4) * 3)));
            gb.WriteBytes(new byte[] { 0, 0x40 }); //Temporary!!
        }

        public void DecompressGroupSeasons(int group, MapLoader mapLoader)
        {
            if (gb.Buffer.Length == 0x100000)
            {
                throw new Exception("ROM has not been expanded.");
            }

            if (group == 0)
            {
                for (int season = 0; season < 4; season++)
                {
                    int address = 0x150000 + (season * 2) * 0x4000; //Bank 0x54
                    for (int i = 0; i < 256; i++)
                    {
                        mapLoader.loadMap(i, group, season, false, Program.GameTypes.Seasons);
                        MapLoader.Room r = mapLoader.room;

                        gb.BufferLocation = address;
                        gb.WriteBytes(r.decompressed);
                        int count = gb.BufferLocation - address;
                        address = gb.BufferLocation;

                        gb.BufferLocation = 0x42 * 0x4000 + season * 0x400 + i * 4;
                        gb.WriteByte(1);
                        gb.WriteByte((byte)((address - count) / 0x4000));
                        gb.WriteBytes(gb.Get2BytePointer(address - count));

                        //Write the new bank indexes
                        /*group = season; //7 + season;
                        gb.BufferLocation = 0x5C * 0x4000 + group * 2;
                        gb.WriteBytes(gb.Get2BytePointer(group * 0x100 + 0x20));
                        gb.BufferLocation = 0x5C * 0x4000 + group * 0x100 + 0x20 + i;
                        gb.WriteByte((byte)((address - count) / 0x4000));
                        //Write the map-specific header
                        gb.BufferLocation = 0x40 * 0x4000 + 0x200 + (0x200 * group) + (i * 2);
                        byte[] b = gb.Get2BytePointer(address - count);
                        gb.WriteBytes(new byte[] { b[0], (byte)(b[1] - 0x40) });
                        group = 0;*/
                        //gb.WriteBytes(new byte[] { (byte)(count + (address % 0x4000)), (byte)((count + (address % 0x4000)) >> 8) });
                    }
                }
            }
            else if (group > 0 && group < 4) //Subrosia
            {
                int address = 0x104000 + (group * 2) * 0x4000; //Bank 0x41
                for (int i = 0; i < 256; i++)
                {
                    mapLoader.loadMap(i, group, 0, false, Program.GameTypes.Seasons);
                    MapLoader.Room r = mapLoader.room;

                    gb.BufferLocation = address;
                    gb.WriteBytes(r.decompressed);
                    int count = gb.BufferLocation - address;
                    address = gb.BufferLocation;

                    gb.BufferLocation = 0x42 * 0x4000 + 4 * 0x400 + i * 4;
                    gb.WriteByte(1);
                    gb.WriteByte((byte)((address - count) / 0x4000));
                    gb.WriteBytes(gb.Get2BytePointer(address - count));
                }
            }
            else
            {
                int address = 0x104000 + (group * 3) * 0x4000; //Bank 0x41
                for (int i = 0; i < 256; i++)
                {
                    mapLoader.loadMap(i, group, 0, false, Program.GameTypes.Seasons);
                    MapLoader.Room r = mapLoader.room;

                    gb.BufferLocation = address;
                    gb.WriteBytes(r.decompressed);
                    int count = gb.BufferLocation - address;
                    address = gb.BufferLocation;

                    gb.BufferLocation = 0x42 * 0x4000 + (group + 1) * 0x400 + i * 4;
                    gb.WriteByte(0);
                    gb.WriteByte((byte)((address - count) / 0x4000));
                    gb.WriteBytes(gb.Get2BytePointer(address - count));
                }
            }

            //Write the new region header
            int indexBase = 0x10C4C;
            if (group == 0)
            {
                for (int season = 0; season < 4; season++)
                {
                    byte a = (byte)(season * 8);
                    gb.BufferLocation = indexBase + a;
                    gb.WriteByte(1); //The map type
                    gb.WriteByte(0x40); //The header pointers will always be in bank 0x40
                    gb.WriteBytes(new byte[] { 0, (byte)(0x42 + (0x4 * season)) }); // The pointer. 0x300 bytes per group
                    //gb.WriteByte((byte)(0x41 + (season * 2)));
                    //gb.WriteBytes(new byte[] { 0, 0x40 }); //Temporary!!
                }
            }
            else
            {
                byte a = 0x20;
                if (group > 3)
                {
                    a += (byte)((group - 3) * 8);
                }
                gb.BufferLocation = indexBase + a;
                gb.WriteByte((byte)(group < 4 ? 1 : 0)); //The map type
                gb.WriteByte(0x40); //The header pointers will always be in bank 0x40
                gb.WriteBytes(new byte[] { 0, (byte)(0x42 + (0x4 * (a / 8))) }); // The pointer. 0x400 bytes per group
                //else
                //	gb.WriteBytes(new byte[] { 0, (byte)(0x32 + (0x3 * (a / 8))) }); //The relative minor pointer. Only 41 because the loading procedure address 0x1000
                /*if (group < 4)
                    gb.WriteByte((byte)(0x41 + ((a / 8) * 2)));
                else
                    gb.WriteByte((byte)(0x46 + (((a / 8) - 4) * 3)))*/
                //gb.WriteBytes(new byte[] { 0, 0x40 }); //Temporary!!
            }
        }

        public void decompressAllGroups(MapLoader m, Program.GameTypes game)
        {
            if (game == Program.GameTypes.Ages)
            {
                for (int i = 0; i < 6; i++)
                    decompressGroup(i, m, game);
            }
            else
            {
                /*for (int i = 0; i < 6; i++)
                {
                    DecompressGroupSeasons(i, m);
                }*/
                DecompressGroupSeasons(0, m);
                DecompressGroupSeasons(1, m);
                DecompressGroupSeasons(4, m);
                DecompressGroupSeasons(5, m);
            }
        }

        public void saveMusic(int map, int group, byte music, Program.GameTypes game)
        {
            gb.BufferLocation = (game == Program.GameTypes.Ages ? 0x1095C : 0x1083C) + group * 2;
            gb.BufferLocation = 0x10000 + gb.ReadByte() + ((gb.ReadByte() - 0x40) * 0x100);
            gb.BufferLocation += map;
            gb.WriteByte(music);
        }

        public void saveAreaData(int index, int vram, int tileset, int unique, int animation, int palette, int season, Program.GameTypes game)
        {
            int indexBase = 0x10000 + (game == Program.GameTypes.Ages ? 0x0F9C : 0x0C84);
            gb.BufferLocation = indexBase + index * 8;
            if (game == Program.GameTypes.Seasons)
            {
                if (gb.ReadByte() == 0xFF)
                {
                    gb.BufferLocation = 0x10000 + gb.ReadByte() + ((gb.ReadByte() - 0x40) * 0x100);
                    gb.BufferLocation += season * 8;
                }
                else
                    gb.BufferLocation--;
            }
            gb.BufferLocation += 2;
            gb.WriteByte((byte)unique);
            gb.WriteByte((byte)vram);
            gb.WriteByte((byte)palette);
            gb.WriteByte((byte)tileset);
            gb.BufferLocation++;
            gb.WriteByte((byte)animation);
        }

        public void saveAreaIndex(int index, int group, byte newArea, Program.GameTypes game)
        {
            int indexBase = 0x10000 + (game == Program.GameTypes.Ages ? 0x0F9C : 0x0C84);
            //4:6d7a - start of procedure
            gb.BufferLocation = 0x10000 + (game == Program.GameTypes.Ages ? 0x12D4 : 0x133C) + (group * 2);
            //6da1
            gb.BufferLocation = 0x10000 + gb.ReadByte() + ((gb.ReadByte() - 0x40) * 0x100) + index;
            gb.WriteByte(newArea);
        }
    }
}
