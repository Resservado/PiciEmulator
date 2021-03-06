﻿using System.Collections;
using System.Collections.Generic;
using Pici.HabboHotel.Items;
using Pici.HabboHotel.Rooms.Games;
using Pici.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Pici.Storage.Database.Session_Details.Interfaces;
using System.Data;
using System;

namespace Pici.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    class ToggleItemState: IWiredTrigger, IWiredCycleable, IWiredEffect
    {
        private uint itemID;
        private Gamemap gamemap;
        private WiredHandler handler;

        private List<RoomItem> items;
        private int delay;
        private int cycles;
        private Queue delayedTriggeringUsers;

        private bool disposed;

        public ToggleItemState(Gamemap gamemap, WiredHandler handler, List<RoomItem> items, int delay, uint itemID)
        {
            this.gamemap = gamemap;
            this.handler = handler;
            this.items = items;
            this.delay = delay;
            this.cycles = 0;
            this.delayedTriggeringUsers = new Queue();
            this.disposed = false;
        }

        public bool OnCycle()
        {
            if (disposed)
                return false;

            cycles++;
            if (cycles > delay)
            {
                if (delayedTriggeringUsers.Count > 0)
                {
                    lock (delayedTriggeringUsers.SyncRoot)
                    {
                        while (delayedTriggeringUsers.Count > 0)
                        {
                            RoomUser user = (RoomUser)delayedTriggeringUsers.Dequeue();
                            ToggleItems(user);
                        }
                    }
                }
                return false;
            }

            return true;
        }

        public bool Handle(RoomUser user, Team team, RoomItem item)
        {
            if (disposed)
                return false;
            cycles = 0;
            if (delay == 0 && user != null)
            {
                return ToggleItems(user);
            }
            else
            {
                lock (delayedTriggeringUsers.SyncRoot)
                {
                    delayedTriggeringUsers.Enqueue(user);
                }
                handler.RequestCycle(this);
            }
            return false;
        }

        private bool ToggleItems(RoomUser user)
        {
            if (disposed)
                return false;
            handler.OnEvent(itemID);
            bool itemTriggered = false;

            foreach (RoomItem item in items)
            {
                if (item == null)
                    continue;
                if (user != null && user.GetClient() != null)
                    item.Interactor.OnTrigger(user.GetClient(), item, 0, true);
                else
                    item.Interactor.OnTrigger(null, item, 0, true);
                itemTriggered = true;
            }
            return itemTriggered;
        }

        public void Dispose()
        {
            disposed = true;
            gamemap = null;
            handler = null;
            if (items != null)
                items.Clear();
            delayedTriggeringUsers.Clear();
        }

        public bool IsSpecial(out SpecialEffects function)
        {
            function = SpecialEffects.None;
            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, (int)itemID, "integer", string.Empty, delay.ToString(), false);
            lock (items)
            {
                dbClient.runFastQuery("DELETE FROM trigger_in_place WHERE original_trigger = '" + this.itemID + "'"); 
                foreach (RoomItem i in items)
                {
                    WiredUtillity.SaveTrigger(dbClient, (int)itemID, (int)i.Id);
                }
            }
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.setQuery("SELECT trigger_data FROM trigger_item WHERE trigger_id = @id ");
            dbClient.addParameter("id", (int)this.itemID);
            DataRow dRow = dbClient.getRow();
            if (dRow != null)
                this.delay = Convert.ToInt32(dRow[0].ToString());
            else
                this.delay = 20;

            dbClient.setQuery("SELECT triggers_item FROM trigger_in_place WHERE original_trigger = " + this.itemID);
            DataTable dTable = dbClient.getTable();
            RoomItem targetItem;
            foreach (DataRow dRows in dTable.Rows)
            {
                targetItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToUInt32(dRows[0]));
                if (targetItem == null || this.items.Contains(targetItem))
                    continue;
                this.items.Add(targetItem);
            }
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.runFastQuery("DELETE FROM trigger_item WHERE trigger_id = '" + this.itemID + "'");
            dbClient.runFastQuery("DELETE FROM trigger_in_place WHERE original_trigger = '" + this.itemID + "'");
        }

        public bool Disposed()
        {
            return disposed;
        }
    }
}
