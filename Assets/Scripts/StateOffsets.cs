using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Globalization;

public class StateOffsets : MonoBehaviour
{
    public ulong playerStateAddress = 0x80453080;

    public Dictionary<string, ulong> dataOffsets;
    // Start is called before the first frame update
    void Awake()
    {
        dataOffsets = new()
        {
            //From netplay address
                //At index 0: 0 = not in online, 32 after going online once
                ["Connection State"] = 0x00,
                ["Local Player Ready"] = 0x01,
                ["Remote Player Ready"] = 0x02,
                ["Local Player Index"] = 0x03,
                ["Remote Player Index"] = 0x04,

				// [0x05] = { type = "u32", name = "rng_offset" },						
				// [0x09] = { type = "u8", name = "delay_frames" },						
				// [0x0A] = { type = "u8", name = "local_player.chatmsg_id" },				
				// [0x0B] = { type = "u8", name = "opponent.chatmsg_id" },					
				// [0x0C] = { type = "u8", name = "chatmsg.index" },						
				// [0x0D] = { type = "u32", name = "vs.left_names" },						
				// [0x11] = { type = "u32", name = "vs.right_names" },	

            //From Static Player Block
                //Data at index 1
                ["HP Lost / Percentage"] = 0x60,

                //Data at index 3
                ["FallOnGroundNoTech"] = 0x680,
                ["Falls"] = 0x68,

                //Data at unknown
                ["Getup Attack U"] = 0x594,
                ["Getup Attack D"] = 0x598,
                ["Dash Attack"] = 0x4e4,

                //Pointer to the Player Entity Struct
                ["PlayerEntityPointer"] = 0xB0,

                    //From Player Entity Struct
                    //Pointer to the Character Data Struct
                    ["CharacterDataPointer"] = 0x2c,
                        //From Character Data Struct
                        //Facing Left: Index 0 = 191, Facing right: Index 0 = 63
                        ["FacingDirection"] = 0x2c,
                        //Index 0: no jumps used = 0, first jump = 1, double jump = 2
                        ["Number Of Jumps Used"] = 0x1968,

                        //Definitly something to do with hitting (values are on hit, else 0 (index 0 and 1))
                        ["Hitlag Counter"] = 0x195C,

                        //Doesn't seem to be anything
                        ["Percentage"] = 0x1830,

                        //["Hitstun"] = 0x221C,
                        //Lot of weird values but: At Index 0: Not in hitstun = 0, in hitstun = 128
                        ["InHitstun"] = 0x21D8
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ulong AddOffsetToAddress(string _state, ulong _address)
    {

        if(_address == 0x80453080)
        {
            for(int i = 0; i < GameData.port; i++)
            {
                _address += (ulong)i * 0xE90;
            }
        }

        _address += dataOffsets[_state];

        return _address;
    }

    public ulong GetAddressFromPointer(byte[] memory, int offset = 0, bool isBigEndian = true)
    {
        string memoryAsString = BitConverter.ToString(memory);
        UnityEngine.Debug.Log("Memory as string: " + memoryAsString);

        string cleanedInput = memoryAsString.Replace("-", "");
        UnityEngine.Debug.Log("Cleaned input: " + cleanedInput);
        
        ulong address = ulong.Parse(cleanedInput, NumberStyles.HexNumber);

        UnityEngine.Debug.Log("Memory as address: " + address);

        // Convert bytes to ulong (pointer value)
        return address;
    }

}
