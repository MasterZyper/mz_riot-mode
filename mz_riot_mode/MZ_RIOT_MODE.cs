/*
MZ-Riot-Mode, put all NPCs in panic.
Copyright (C) 27.08.2019 MasterZyper 🐦
Contact: masterzyper@reloaded-server.de
You like to get a FiveM-Server? 
Visit ZapHosting*: https://zap-hosting.com/a/17444fc14f5749d607b4ca949eaf305ed50c0837

Support us on Patreon: https://www.patreon.com/gtafivemorg

For help with this Script visit: https://gta-fivem.org/

This program is free software; you can redistribute it and/or modify it under the terms of the 
GNU General Public License as published by the Free Software Foundation; either version 3 of 
the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with this program; 
if not, see <http://www.gnu.org/licenses/>.

*Affiliate-Link: Euch entstehen keine Kosten oder Nachteile. Kauf über diesen Link erwirtschaftet eine kleine prozentuale Provision für mich.
*/

using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mz_riot_mode
{
    public class MZ_RIOT_MODE : BaseScript
    {
        readonly Random rand = new Random();
        readonly WeaponHash[] SmallWeapons = new WeaponHash[] {
            WeaponHash.BZGas,
            WeaponHash.FlareGun,
            WeaponHash.GrenadeLauncherSmoke,
            WeaponHash.Molotov,
            WeaponHash.PipeBomb,
            WeaponHash.ProximityMine,
            WeaponHash.SmokeGrenade,
            WeaponHash.SMG,
            WeaponHash.SMGMk2,
            WeaponHash.Revolver,
            WeaponHash.RevolverMk2,
            WeaponHash.CombatPistol,
            WeaponHash.Pistol,
            WeaponHash.Pistol50,
            WeaponHash.PistolMk2,
            WeaponHash.RayPistol,
            WeaponHash.BattleAxe,
            WeaponHash.Bottle,
            WeaponHash.Bat,
            WeaponHash.GolfClub,
            WeaponHash.Hammer,
            WeaponHash.Hatchet,
            WeaponHash.Knife,
            WeaponHash.KnuckleDuster,
            WeaponHash.Machete,
            WeaponHash.Nightstick
        };
        readonly WeaponHash[] BigWeapons = new WeaponHash[] {
            WeaponHash.AssaultRifle,
            WeaponHash.AssaultShotgun,
            WeaponHash.BullpupRifle,
            WeaponHash.CompactGrenadeLauncher,
            WeaponHash.Firework,
            WeaponHash.GrenadeLauncher,
            WeaponHash.GrenadeLauncherSmoke,
            WeaponHash.Hatchet,
            WeaponHash.HomingLauncher,
            WeaponHash.MG,
            WeaponHash.RayMinigun,
            WeaponHash.RayCarbine,
            WeaponHash.RayPistol,
            WeaponHash.Snowball,
            WeaponHash.SweeperShotgun,
            WeaponHash.PetrolCan,
            WeaponHash.FireExtinguisher,
            WeaponHash.AdvancedRifle,
            WeaponHash.Ball
        };
       readonly VehicleHash[] PlanHashes = new VehicleHash[]{
            VehicleHash.Shamal,
            VehicleHash.Jet,
            VehicleHash.Besra,
            VehicleHash.CargoPlane,
            VehicleHash.Titan,
            VehicleHash.Luxor,
            VehicleHash.Luxor2,
            VehicleHash.Cuban800,
            VehicleHash.Dodo,
            VehicleHash.Duster,
            VehicleHash.Hydra,
            VehicleHash.Jet,
            VehicleHash.Mammatus,
            VehicleHash.Miljet,
            VehicleHash.Nimbus,
            VehicleHash.Stunt,
            VehicleHash.Velum,
            VehicleHash.Velum2,
            VehicleHash.Vestra
        };
        public bool random_plane_fights = true;
        public MZ_RIOT_MODE()
        {
            string resource_name = API.GetCurrentResourceName();
            string resource_author = "MasterZyper";
            if (Convert.ToBoolean(Convert.ToInt32(API.GetResourceMetadata(API.GetCurrentResourceName(), "riot_mode", 0))))
            {
                Tick += RiotModeTick;
                random_plane_fights = Convert.ToBoolean(Convert.ToInt32(API.GetResourceMetadata(API.GetCurrentResourceName(), "random_plane_fights", 0)));
            }
            Debug.Write($"{resource_name} by {resource_author} was started sucessfully");
        }
        private Ped GetRandomPed()
        {
            Ped[] peds = World.GetAllPeds();
            return peds[rand.Next(0, peds.Length)];
        }
        private Ped FindPedNearbyPed(Ped target)
        {
            Ped[] peds = World.GetAllPeds();
            Ped nextPed = Game.PlayerPed;
            float dist = float.MaxValue;
            foreach (Ped ped in peds)
            {
                if (ped != target && !ped.IsDead)
                {
                    float new_dist = World.GetDistance(target.Position, ped.Position);
                    if (new_dist < dist + 5)
                    {
                        dist = new_dist;
                        nextPed = ped;
                    }
                }
            }
            return nextPed;
        }
        private void GivePedRandomWeapon(Ped ped)
        {
            ped.Weapons.Give(SmallWeapons[rand.Next(0, SmallWeapons.Length)], 1000, true, true);
            ped.Weapons.Give(BigWeapons[rand.Next(0, BigWeapons.Length)], 1000, true, true);
            ped.Weapons.Current.InfiniteAmmo = true;
        }
        private VehicleHash GenerateRandomPlaneHash()
        {
            return PlanHashes[rand.Next(0, PlanHashes.Length)];
        }
        private async Task<Vehicle> GenerateRandomPlaneScenarioInAir(Vector3 pos)
        {
            float GroundZ = World.GetGroundHeight(pos);
            Vector3 position = new Vector3(pos.X, pos.Y, GroundZ + 150);
            Vector3 landing_position = new Vector3(pos.X + rand.Next(-5000, 5000), pos.Y + rand.Next(-5000, 5000), GroundZ + 150);
            Ped pilot = await World.CreatePed(PedHash.Andreas, position);
            Vehicle plane = await World.CreateVehicle(GenerateRandomPlaneHash(), position);
            plane.Rotation = new Vector3(0, 0, rand.Next(0, 360));
            plane.Speed = 100f;
            plane.LandingGearState = VehicleLandingGearState.Closing;
            pilot.Task.WarpIntoVehicle(plane, VehicleSeat.Driver);
            await Delay(10);
            pilot.Task.LandPlane(landing_position, landing_position, plane);
            pilot.DrivingStyle = DrivingStyle.IgnorePathing;
            Vector3 position2 = new Vector3(pos.X - 50, pos.Y + 100, GroundZ + 100);
            Ped pilot2 = await World.CreatePed(PedHash.Armymech01SMY, position);
            Vehicle plane2 = await World.CreateVehicle(GenerateRandomPlaneHash(), position2);
            plane2.Rotation = new Vector3(0, 0, rand.Next(0, 360));
            plane2.Speed = 90f;
            plane2.LandingGearState = VehicleLandingGearState.Closing;
            pilot2.Task.WarpIntoVehicle(plane2, VehicleSeat.Driver);
            await Delay(10);
            API.SetPedCombatAttributes(pilot2.Handle, 3, false);
            pilot2.Task.ChaseWithPlane(plane, new Vector3(150, 150, 150));
            pilot2.DrivingStyle = DrivingStyle.IgnorePathing;
            pilot2.Task.FightAgainst(pilot);
            return plane;
        }
        private async Task RiotModeTick()
        {
            if (rand.Next(0, 5000) == 5)
            {
                if (random_plane_fights)
                {
                    await GenerateRandomPlaneScenarioInAir(Game.PlayerPed.Position);
                }
            }
            Ped[] peds = World.GetAllPeds();
            foreach (Ped ped in peds)
            {
                if (!ped.IsHuman)
                {
                    ped.Delete();
                }
                if (!ped.IsPlayer && !ped.IsDead && !ped.IsInPlane)
                {
                    if (!ped.IsInCombat)
                    {
                        ped.Task.FightAgainst(FindPedNearbyPed(ped));
                    }
                    if (ped.Weapons.Current.Hash == WeaponHash.Unarmed)
                    {
                        if (rand.Next(0, 3) != 1)
                        {
                            GivePedRandomWeapon(ped);
                            if (rand.Next(0, 2) == 1)
                            {
                                API.SetPedFleeAttributes(ped.Handle, 0, true);
                            }
                            else
                            {
                                API.SetPedFleeAttributes(ped.Handle, 0, false);
                            }
                            if (rand.Next(0, 5) == 1)
                            {
                                API.SetPedCombatAttributes(ped.Handle, 1, false);
                            }
                            if (rand.Next(0, 5) == 1)
                            {
                                API.SetPedCombatAttributes(ped.Handle, 3, true);
                            }
                            API.SetPedCombatAttributes(ped.Handle, 2, true);

                            API.SetPedCombatAttributes(ped.Handle, 16, true);
                            API.SetPedCombatAttributes(ped.Handle, 17, false);
                            API.SetPedCombatAttributes(ped.Handle, 46, true);    //Immer angreifen
                            API.SetPedCombatAttributes(ped.Handle, 5, true);     //Angreifen wenn er nicht bewaffnet ist aber der Spieler
                            API.SetPedCombatMovement(ped.Handle, 3);
                            API.SetPedCombatRange(ped.Handle, 2);
                            API.SetPedAlertness(ped.Handle, 3);
                        }
                    }
                    if (ped.IsInVehicle())
                    {
                        Vehicle veh = ped.CurrentVehicle;

                        VehicleDrivingFlags flags =
                            VehicleDrivingFlags.AllowGoingWrongWay |
                            VehicleDrivingFlags.AllowMedianCrossing |
                            VehicleDrivingFlags.IgnorePathFinding;
                        ped.DrivingStyle = (DrivingStyle)flags;
                        API.SetDriveTaskMaxCruiseSpeed(ped.Handle, 250f);
                        API.SetDriveTaskCruiseSpeed(ped.Handle, 250f);
                        veh.MaxSpeed = 200;
                        ped.MaxDrivingSpeed = 20000;
                        ped.DrivingSpeed = 20000;
                        ped.CanBeShotInVehicle = true;
                        ped.CanFlyThroughWindscreen = true;
                    }
                }
            }
            await Delay(0);
        }
    }
}
