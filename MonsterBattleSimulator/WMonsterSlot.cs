/*
	Copyright (c) 2017 Thomas Schöngrundner

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.
*/

using ConUI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterBattleSimulator
{
    public class WMonsterSlot : WButton
    {
        //---------------------------------------------------------------PRIVATE MEMBER VARIABLES----------------------------------------------------------------------
        private MonsterData monster = null;

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        public MonsterData Monster
        {
            get { return monster; }
            set
            {
                monster = value;

                if(monster == null)
                {
                    Text.Text = "Empty Slot / Create Monster";
                }
                else
                {
                    Text.Text = value.Name;
                }
            }
        }

        public WMonsterSlot SetMonster(MonsterData m)
        {
            Monster = m;

            return this;
        }

        public override void OnConstruction()
        {
            //Text.Text = "Empty Slot / Create Monster";
            Text.SetTextAlignment(WText.ETextAlignment.ETEXT_ALIGNMENT_CENTER);

            base.OnConstruction();
        }
    }
}
