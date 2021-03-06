﻿using System;
using System.Collections.Generic;

using Pici.HabboHotel.GameClients;
using Pici.HabboHotel.Pathfinding;
using Pici.HabboHotel.Rooms;
using Pici.HabboHotel.Rooms.Games;
using System.Drawing;
using Pici.Messages;

namespace Pici.HabboHotel.Items.Interactors
{
    abstract class FurniInteractor
    {
        internal abstract void OnPlace(GameClient Session, RoomItem Item);
        internal abstract void OnRemove(GameClient Session, RoomItem Item);
        internal abstract void OnTrigger(GameClient Session, RoomItem Item, int Request, Boolean UserHasRights);
    }

    //class InteractorStatic : FurniInteractor
    //{
    //    internal override void OnPlace(GameClient Session, RoomItem Item) { }
    //    internal override void OnRemove(GameClient Session, RoomItem Item) { }
    //    internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights) { }
    //}

    class InteractorTeleport : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.ClearMovement(true);
                    User.AllowOverride = false;
                    User.CanWalk = true;
                }

                Item.InteractingUser = 0;
            }

            if (Item.InteractingUser2 != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser2);

                if (User != null)
                {
                    User.ClearMovement(true);
                    User.AllowOverride = false;
                    User.CanWalk = true;
                }

                Item.InteractingUser2 = 0;
            }

            //Item.GetRoom().RegenerateUserMatrix();
        }

        internal override void OnRemove(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.UnlockWalking();
                }

                Item.InteractingUser = 0;
            }

            if (Item.InteractingUser2 != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser2);

                if (User != null)
                {
                    User.UnlockWalking();
                }

                Item.InteractingUser2 = 0;
            }

            //Item.GetRoom().RegenerateUserMatrix();
        }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            // Is this user valid?
            if (Item == null || Item.GetRoom() == null || Session == null || Session.GetHabbo() == null)
                return;
            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            // Alright. But is this user in the right position?
            if (User.Coordinate == Item.Coordinate || User.Coordinate == Item.SquareInFront)
            {
                // Fine. But is this tele even free?
                if (Item.InteractingUser != 0)
                {
                    return;
                }

                //User.TeleDelay = -1;
                Item.InteractingUser = User.GetClient().GetHabbo().Id;
            }
            else if (User.CanWalk)
            {
                User.MoveTo(Item.SquareInFront);
            }
        }
    }

    class InteractorSpinningBottle : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";
            Item.UpdateState(true, false);
        }

        internal override void OnRemove(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";
        }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Item.ExtraData != "-1")
            {
                Item.ExtraData = "-1";
                Item.UpdateState(false, true);
                Item.ReqUpdate(3, true);
            }
        }
    }

    class InteractorDice : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            RoomUser User = null;
            if (Session != null)
                User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
            {
                if (Item.ExtraData != "-1")
                {
                    if (Request == -1)
                    {
                        Item.ExtraData = "0";
                        Item.UpdateState();
                    }
                    else
                    {
                        Item.ExtraData = "-1";
                        Item.UpdateState(false, true);
                        Item.ReqUpdate(4, true);
                    }
                }
            }
            else
            {
                User.MoveTo(Item.SquareInFront);
            }
        }
    }

    class InteractorHabboWheel : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "-1";
            Item.ReqUpdate(10, true);
        }

        internal override void OnRemove(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "-1";
        }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            if (Item.ExtraData != "-1")
            {
                Item.ExtraData = "-1";
                Item.UpdateState();
                Item.ReqUpdate(10, true);
            }
        }
    }

    class InteractorLoveShuffler : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "-1";
        }

        internal override void OnRemove(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "-1";
        }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            if (Item.ExtraData != "0")
            {
                Item.ExtraData = "0";
                Item.UpdateState(false, true);
                Item.ReqUpdate(10, true);
            }
        }
    }

    class InteractorOneWayGate : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.ClearMovement(true);
                    User.UnlockWalking();
                }

                Item.InteractingUser = 0;
            }
        }

        internal override void OnRemove(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.ClearMovement(true);
                    User.UnlockWalking();
                }

                Item.InteractingUser = 0;
            }
        }
        
        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Session == null)
                return;
            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            if (User.Coordinate != Item.SquareInFront && User.CanWalk)
            {
                User.MoveTo(Item.SquareInFront);
                return;
            }

            if (!Item.GetRoom().GetGameMap().CanWalk(Item.SquareBehind.X, Item.SquareBehind.Y, User.AllowOverride))
            {
                return;
            }

            if (Item.InteractingUser == 0)
            {
                Item.InteractingUser = User.HabboId;

                User.CanWalk = false;

                if (User.IsWalking && (User.GoalX != Item.SquareInFront.X || User.GoalY != Item.SquareInFront.Y))
                {
                    User.ClearMovement(true);
                }

                User.AllowOverride = true;
                User.MoveTo(Item.Coordinate);

                Item.ReqUpdate(4, true);
            }
        }
    }

    class InteractorAlert : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";
        }

        internal override void OnRemove(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";
        }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            if (Item.ExtraData == "0")
            {
                Item.ExtraData = "1";
                Item.UpdateState(false, true);
                Item.ReqUpdate(4, true);
            }
        }
    }

    class InteractorVendor : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser > 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.CanWalk = true;
                }
            }
        }
       
        internal override void OnRemove(GameClient Session, RoomItem Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser > 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.CanWalk = true;
                }
            }
        }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Item.ExtraData != "1" && Item.GetBaseItem().VendingIds.Count >= 1 && Item.InteractingUser == 0 && Session != null)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User == null)
                {
                    return;
                }

                if (!Gamemap.TilesTouching(User.X, User.Y, Item.GetX, Item.GetY))
                {
                    User.MoveTo(Item.SquareInFront);
                    return;
                }

                Item.InteractingUser = Session.GetHabbo().Id;

                User.CanWalk = false;
                User.ClearMovement(true);
                User.SetRot(Rotation.Calculate(User.X, User.Y, Item.GetX, Item.GetY), false);

                Item.ReqUpdate(2, true);

                Item.ExtraData = "1";
                Item.UpdateState(false, true);
            }
        }
    }

    class InteractorGenericSwitch : FurniInteractor
    {
        int Modes;

        internal InteractorGenericSwitch(int Modes)
        {
            this.Modes = (Modes - 1);

            if (this.Modes < 0)
            {
                this.Modes = 0;
            }
        }

        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Session != null)
                PiciEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, HabboHotel.Quests.QuestType.FURNI_SWITCH);
            if (!UserHasRights)
            {
                return;
            }

            if (this.Modes == 0)
            {
                return;
            }

            int currentMode = 0;
            int newMode = 0;

            try
            {
                currentMode = int.Parse(Item.ExtraData);
            }
            catch // (Exception e)
            {
                //Logging.HandleException(e, "InteractorGenericSwitch.OnTrigger");
            }

            if (currentMode <= 0)
            {
                newMode = 1;
            }
            else if (currentMode >= Modes)
            {
                newMode = 0;
            }
            else
            {
                newMode = currentMode + 1;
            }

            Item.ExtraData = newMode.ToString();
            Item.UpdateState();
        }
    }

    class InteractorNone : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
        }
    }


    class InteractorGate : FurniInteractor
    {
        int Modes;

        internal InteractorGate(int Modes)
        {
            this.Modes = (Modes - 1);

            if (this.Modes < 0)
            {
                this.Modes = 0;
            }
        }

        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            InteractionType type = Item.GetBaseItem().InteractionType;
            if (type != Pici.HabboHotel.Items.InteractionType.gate)
                return;

            if (this.Modes == 0)
            {
                Item.UpdateState(false, true);
            }

            int currentMode = 0;
            int newMode = 0;

            try
            {
                currentMode = int.Parse(Item.ExtraData);
            }
            catch //(Exception e)
            {
                //Logging.HandleException(e, "InteractorGate.OnTrigger");
            }

            if (currentMode <= 0)
            {
                newMode = 1;
            }
            else if (currentMode >= Modes)
            {
                newMode = 0;
            }
            else
            {
                newMode = currentMode + 1;
            }

            if (newMode == 0)
            {
                if (!Item.GetRoom().GetGameMap().itemCanBePlacedHere(Item.GetX, Item.GetY))
                {
                    return;
                }

                //Dictionary<int, ThreeDCoord> Points = Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width,
                //    Item.GetX, Item.GetY, Item.Rot);

                //if (Points == null)
                //{
                //    Points = new Dictionary<int, ThreeDCoord>();
                //}

                //foreach (Rooms.AffectedTile Tile in Points.Values)
                //{
                //    if (!Item.GetRoom().SquareIsOpen(Tile.X, Tile.Y, false))
                //        return;
                //}
            }

            Item.ExtraData = newMode.ToString();
            Item.UpdateState();
            Item.GetRoom().GetGameMap().updateMapForItem(Item);
            //Item.GetRoom().GenerateMaps();
        }
    }

    class InteractorScoreboard : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            int oldValue = 0;

            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                try
                {
                    oldValue = int.Parse(Item.ExtraData);
                }
                catch { }
            }


            if (Request == 1)
            {
                if (Item.pendingReset && oldValue > 0)
                {
                    oldValue = 0;
                    Item.pendingReset = false;
                }
                else {
                    oldValue = oldValue + 60;
                    Item.UpdateNeeded = false;
                }
            }
            else if (Request == 2)
            {
                Item.UpdateNeeded = !Item.UpdateNeeded;
                Item.pendingReset = true;
            }


            Item.ExtraData = oldValue.ToString();
            Item.UpdateState();
        }
    }

    class InteractorFootball : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Session == null)
                return;
            RoomUser interactingUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            Point userCoord = interactingUser.Coordinate;
            Point ballCoord = Item.Coordinate;

            int differenceX = userCoord.X - ballCoord.X;
            int differenceY = userCoord.Y - ballCoord.Y;

            if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1)
            {
                differenceX = differenceX * 2;
                differenceY = differenceY * 2;

                int newX = Item.GetX + differenceX;
                int newY = Item.GetY + differenceY;

                Item.GetRoom().GetSoccer().MoveBall(Item, Session, newX, newY);
                interactingUser.MoveTo(ballCoord);
            }
            else //if (differenceX == 2 || differenceY == 2 || differenceY == - 2 || differenceX == -2)
            {
                Item.interactingBallUser = Session.GetHabbo().Id;

                differenceX = differenceX * (-1);
                differenceY = differenceY * (-1);

                if (differenceX > 1)
                    differenceX = 1;
                else if (differenceX < -1)
                    differenceX = -1;


                if (differenceY > 1)
                    differenceY = 1;
                else if (differenceY < -1)
                    differenceY = -1;


                int newX = Item.GetX + differenceX;
                int newY = Item.GetY + differenceY;

                interactingUser.MoveTo(new Point(newX, newY));
            }
        }
    }

    class InteractorScoreCounter : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            if (Item.team == Team.none)
                return;

            Item.ExtraData = Item.GetRoom().GetGameManager().Points[(int)Item.team].ToString();
            Item.UpdateState(false, true);
        }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            int oldValue = 0;

            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                try
                {
                    oldValue = int.Parse(Item.ExtraData);
                }
                catch { }
            }


            if (Request == 1)
            {
                oldValue++;
            }
            else if (Request == 2)
            {
                oldValue--;
            }
            else if (Request == 3)
            {
                oldValue = 0;
            }

            Item.ExtraData = oldValue.ToString();
            Item.UpdateState(false, true);
        }

        private static void UpdateTeamPoints(int points, Team team, RoomItem Item)
        {
            if (team == Team.none)
                return;

            Item.GetRoom().GetGameManager().Points[(int)Item.team] = points;
            Item.UpdateState(false, true);
        }
    }
    class InteractorBanzaiScoreCounter : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item)
        {
            if (Item.team == Team.none)
                return;

            Item.ExtraData = Item.GetRoom().GetGameManager().Points[(int)Item.team].ToString();
            Item.UpdateState(false, true);
        }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }


            Item.GetRoom().GetGameManager().Points[(int)Item.team] = 0;

            Item.ExtraData = "0";
            Item.UpdateState();
        }

        private static void UpdateTeamPoints(int points, Team team, RoomItem Item)
        {
            if (team == Team.none)
                return;

            Item.GetRoom().GetGameManager().Points[(int)Item.team] = points;
            Item.UpdateState();
        }
    }



    class InteractorBanzaiTimer : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            int oldValue = 0;

            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                try
                {
                    oldValue = int.Parse(Item.ExtraData);
                }
                catch { }
            }

            if (Request == 2)
            {
                if (Item.pendingReset && oldValue > 0)
                {
                    oldValue = 0;
                    Item.pendingReset = false;
                }
                else
                {
                    oldValue = oldValue + 60;
                    Item.UpdateNeeded = false;
                }
            }
            else if (Request == 1)
            {
                if (!Item.GetRoom().GetBanzai().isBanzaiActive)
                {
                    Item.UpdateNeeded = !Item.UpdateNeeded;

                    if (Item.UpdateNeeded)
                    {
                        //Console.WriteLine("Game started");
                        Item.GetRoom().GetBanzai().BanzaiStart();
                    }

                    Item.pendingReset = true;
                }
            }


            Item.ExtraData = oldValue.ToString();
            Item.UpdateState();
        }
    }

    class InteractorBanzaiPuck : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Session == null)
                return;
            RoomUser interactingUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            Point userCoord = interactingUser.Coordinate;
            Point ballCoord = Item.Coordinate;

            int differenceX = userCoord.X - ballCoord.X;
            int differenceY = userCoord.Y - ballCoord.Y;

            if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1)
            {
                differenceX = differenceX * 2;
                differenceY = differenceY * 2;

                int newX = Item.GetX + differenceX;
                int newY = Item.GetY + differenceY;

                Item.GetRoom().GetSoccer().MoveBall(Item, Session, newX, newY);
                interactingUser.MoveTo(ballCoord);
            }
            else //if (differenceX == 2 || differenceY == 2 || differenceY == - 2 || differenceX == -2)
            {
                Item.interactingBallUser = Session.GetHabbo().Id;

                differenceX = differenceX * (-1);
                differenceY = differenceY * (-1);

                if (differenceX > 1)
                    differenceX = 1;
                else if (differenceX < -1)
                    differenceX = -1;


                if (differenceY > 1)
                    differenceY = 1;
                else if (differenceY < -1)
                    differenceY = -1;


                int newX = Item.GetX + differenceX;
                int newY = Item.GetY + differenceY;

                interactingUser.MoveTo(new Point(newX, newY));
            }
        }
    }

    class InteractorFreezeTimer : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            int oldValue = 0;

            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                try
                {
                    oldValue = int.Parse(Item.ExtraData);
                }
                catch { }
            }

            if (Request == 2)
            {
                if (Item.pendingReset && oldValue > 0)
                {
                    oldValue = 0;
                    Item.pendingReset = false;
                }
                else
                {
                    oldValue = oldValue + 60;
                    Item.UpdateNeeded = false;
                }
            }
            else if (Request == 1)
            {
                if (Item.pendingReset)
                {
                    oldValue = 0;
                    Item.pendingReset = false;
                }
                else
                {
                    Item.UpdateNeeded = !Item.UpdateNeeded;
                    Item.pendingReset = true;
                }
            }


            Item.ExtraData = oldValue.ToString();
            Item.UpdateState();
        }
    }

    class InteractorFreezeTile : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item.InteractingUser > 0)
                return;

            string username = Session.GetHabbo().Username;
            RoomUser user = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(username);

            user.GoalX = Item.GetX;
            user.GoalY = Item.GetY;

            if (user.team != Team.none)
                user.throwBallAtGoal = true;

            //Console.WriteLine(Request.ToString());

            //int oldValue = 0;

            //if (!string.IsNullOrEmpty(Item.ExtraData))
            //{
            //    try
            //    {
            //        oldValue = int.Parse(Item.ExtraData);
            //    }
            //    catch { }
            //}
            //if (oldValue == 0)
            //    oldValue = 1000;
            //    //oldValue = 11000;
            //else
            //    oldValue = 0;

            //Item.ExtraData = oldValue.ToString();
            //Item.UpdateState();
        }
    }

    class InteractorIncrementer : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {

            int oldValue = 0;

            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                try
                {
                    oldValue = int.Parse(Item.ExtraData);
                }
                catch { }
            }
            oldValue += 1;
            //Console.WriteLine(oldValue.ToString());

            Item.ExtraData = oldValue.ToString();
            Item.UpdateState();
        }
    }

    class InteractorIgnore : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
        }
    }

    class WiredInteractor : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Session == null || Item == null)
                return;

            if (!UserHasRights)
                return;

            switch (Item.GetBaseItem().InteractionType)
            {
                #region Triggers

                case InteractionType.triggerwalkonfurni:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);
                        message.AppendByte(2);

                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggergamestart:
                    {
                        ServerMessage message = new ServerMessage(650);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);
                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggerroomenter:
                    {
                        ServerMessage message = new ServerMessage(650);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(7);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggergameend:
                    {
                        ServerMessage message = new ServerMessage(650);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggertimer:
                    {
                        ServerMessage message = new ServerMessage(650);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(true);
                        message.AppendBoolean(true);
                        message.AppendBoolean(true);
                        message.AppendInt32(3);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggerwalkofffurni:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggeronusersay:
                    {
                        ServerMessage message = new ServerMessage(650);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggerscoreachieved:
                    {
                        ServerMessage message = new ServerMessage(650);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(true);
                        message.AppendInt32(100);
                        message.AppendBoolean(false);
                        message.AppendInt32(10);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggerrepeater:
                    {
                        ServerMessage message = new ServerMessage(650);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(true);
                        message.AppendInt32(10);
                        message.AppendBoolean(false);
                        message.AppendInt32(6);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.triggerstatechanged:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }
                #endregion

                #region Effects
                case InteractionType.actionposreset:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(true);
                        message.AppendUInt(Item.Id);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.actiongivescore:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendInt32(2);
                        message.AppendInt32(5);
                        message.AppendBoolean(true);
                        message.AppendBoolean(false);
                        message.AppendInt32(6);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.actionresettimer:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(true);
                        message.AppendUInt(Item.Id);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.actiontogglestate:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.actionshowmessage:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(7);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.actionteleportto:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.actionmoverotate:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendInt32(2);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendInt32(4);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                #endregion

                #region Add-ons
                case InteractionType.specialrandom:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.specialunseen:
                    {
                        ServerMessage message = new ServerMessage(651);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendByte(2);
                        message.AppendBoolean(false);
                        message.AppendInt32(8);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendByte(2);

                        Session.SendMessage(message);
                        break;
                    }
                #endregion

                #region Conditions
                case InteractionType.conditiontimelessthan:
                case InteractionType.conditiontimemorethan:
                    {
                        ServerMessage message = new ServerMessage(652);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(true);
                        message.AppendUInt(Item.Id);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);
                        //message.AppendBoolean(false);
                        //message.AppendBoolean(false);
                        //message.AppendInt32(7);
                        //message.AppendBoolean(false);
                        //message.AppendBoolean(false);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.conditionfurnishaveusers:
                    {
                        ServerMessage message = new ServerMessage(652);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(true);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.conditionstatepos:
                    {
                        ServerMessage message = new ServerMessage(652);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(true);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);

                        Session.SendMessage(message);
                        break;
                    }

                case InteractionType.conditiontriggeronfurni:
                    {
                        ServerMessage message = new ServerMessage(652);
                        message.AppendBoolean(false);
                        message.AppendInt32(5);
                        message.AppendBoolean(false);
                        message.AppendInt32(Item.GetBaseItem().SpriteId);
                        message.AppendUInt(Item.Id);

                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);
                        message.AppendBoolean(false);

                        Session.SendMessage(message);
                        break;
                    }

                //Unknown:
                //2 radio + 5 selct
                #endregion

            }
        }
    }

    class InteractorJukebox : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Item.ExtraData == "1")
            {
                Item.GetRoom().GetRoomMusicController().Stop();
                Item.ExtraData = "0";
            }
            else
            {
                Item.GetRoom().GetRoomMusicController().Start();
                Item.ExtraData = "1";
            }

            Item.UpdateState();
        }
    }

    class InteractorPuzzleBox : FurniInteractor
    {
        internal override void OnPlace(GameClient Session, RoomItem Item) { }
        internal override void OnRemove(GameClient Session, RoomItem Item) { }

        internal override void OnTrigger(GameClient Session, RoomItem Item, int Request, bool UserHasRights)
        {
            if (Session != null)
                return;
            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            Point ItemCoordx1 = new Point(Item.Coordinate.X + 1, Item.Coordinate.Y);
            Point ItemCoordx2 = new Point(Item.Coordinate.X - 1, Item.Coordinate.Y);
            Point ItemCoordy1 = new Point(Item.Coordinate.X, Item.Coordinate.Y + 1);
            Point ItemCoordy2 = new Point(Item.Coordinate.X, Item.Coordinate.Y - 1);

            if (User == null)
            {
                return;
            }

            if (User.Coordinate != ItemCoordx1 && User.Coordinate != ItemCoordx2 && User.Coordinate != ItemCoordy1 && User.Coordinate != ItemCoordy2)
            {
                if (User.CanWalk)
                {
                    User.MoveTo(Item.SquareInFront);
                    return;
                }
            }
            else
            {
                int NewX = Item.Coordinate.X;
                int NewY = Item.Coordinate.Y;

                if (User.Coordinate == ItemCoordx1)
                {
                    NewX = Item.Coordinate.X - 1;
                    NewY = Item.Coordinate.Y;
                }
                else if (User.Coordinate == ItemCoordx2)
                {
                    NewX = Item.Coordinate.X + 1;
                    NewY = Item.Coordinate.Y;
                }
                else if (User.Coordinate == ItemCoordy1)
                {
                    NewX = Item.Coordinate.X;
                    NewY = Item.Coordinate.Y - 1;
                }
                else if (User.Coordinate == ItemCoordy2)
                {
                    NewX = Item.Coordinate.X;
                    NewY = Item.Coordinate.Y + 1;
                }

                if (Item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                {
                    Double NewZ = Item.GetRoom().GetGameMap().SqAbsoluteHeight(NewX, NewY);

                    ServerMessage Message = new ServerMessage(230);
                    Message.AppendInt32(Item.Coordinate.X);
                    Message.AppendInt32(Item.Coordinate.Y);
                    Message.AppendInt32(NewX);
                    Message.AppendInt32(NewY);
                    Message.AppendInt32(1);
                    Message.AppendUInt(Item.Id);
                    Message.AppendByte(2);
                    Message.AppendStringWithBreak(TextHandling.GetString(NewZ));
                    Message.AppendString("M");
                    Item.GetRoom().SendMessage(Message);

                    Item.GetRoom().GetRoomItemHandler().SetFloorItem(User.GetClient(), Item, NewX, NewY, Item.Rot, false, false, true);
                }
            }
        }
    }  
}
