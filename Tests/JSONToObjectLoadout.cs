/*
 * Copyright © 2021-2023 Robbyxp1 @ github.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */

using NFluent;
using NUnit.Framework;
using QuickJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tests;

namespace JSONTests
{
    [TestFixture(TestOf = typeof(JToken))]

    public partial class JSONToObjectTests
    {

        [Test]
        public void JSONToObjectLoadout()
        {
            JSONFormatter qj = new JSONFormatter();

            qj.Object().V("event", "Loadout").V("Ship", "asp").V("ShipID", 2).V("ShipName", "SAHARA").V("ShipIdent", "BF-1").V("HullValue", 5214890).V("ModulesValue", 19803569).V("HullHealth", 0.987003).V("UnladenMass", 353.799988).V("CargoCapacity", 64).V("MaxJumpRange", 52.060631)
            .Object("FuelCapacity").V("Main", 32.0).V("Reserve", 0.63).Close()
            .V("Rebuy", 1250926)
            .Array("Modules")
            .Object().V("Slot", "TinyHardpoint1").V("Item", "hpt_chafflauncher_tiny").V("On", false).V("Priority", 0).V("AmmoInClip", 1).V("AmmoInHopper", 10).V("Health", 0.990437).V("Value", 7225).Close()
            .Object().V("Slot", "TinyHardpoint2").V("Item", "hpt_plasmapointdefence_turret_tiny").V("On", false).V("Priority", 0).V("AmmoInClip", 12).V("AmmoInHopper", 10000).V("Health", 0.99459).V("Value", 18546).Close()
            .Object().V("Slot", "PaintJob").V("Item", "paintjob_asp_vibrant_red").V("On", true).V("Priority", 1).V("Health", 1.0).Close()
            .Object().V("Slot", "Armour").V("Item", "asp_armour_grade1").V("On", true).V("Priority", 1).V("Health", 1.0).Close()
            .Object().V("Slot", "PowerPlant").V("Item", "int_powerplant_size4_class5").V("On", true).V("Priority", 1).V("Health", 0.98746).V("Value", 1441233).Close()
            .Object().V("Slot", "MainEngines").V("Item", "int_engine_size5_class2").V("On", true).V("Priority", 0).V("Health", 0.980081).V("Value", 189035).Close()
            .Object().V("Slot", "FrameShiftDrive").V("Item", "int_hyperdrive_size5_class5").V("On", true).V("Priority", 0).V("Health", 0.991966).V("Value", 5103953)
            .Object("Engineering").V("Engineer", "Felicity Farseer").V("EngineerID", 300100).V("BlueprintID", 128673694).V("BlueprintName", "FSD_LongRange").V("Level", 5).V("Quality", 0.666)
            .Array("Modifiers")
            .Object().V("Label", "Mass").V("Value", 26.0).V("OriginalValue", 20.0).V("LessIsGood", 1).Close()
            .Object().V("Label", "Integrity").V("Value", 102.0).V("OriginalValue", 120.0).V("LessIsGood", 0).Close()
            .Object().V("Label", "PowerDraw").V("Value", 0.69).V("OriginalValue", 0.6).V("LessIsGood", 1).Close()
            .Object().V("Label", "FSDOptimalMass").V("Value", 1592.430054).V("OriginalValue", 1050.0).V("LessIsGood", 0).Close()
            .Close()
            .Close()
            .Close()
            .Object().V("Slot", "LifeSupport").V("Item", "int_lifesupport_size4_class2").V("On", true).V("Priority", 0).V("Health", 0.994892).V("Value", 28373).Close()
            .Object().V("Slot", "PowerDistributor").V("Item", "int_powerdistributor_size4_class2").V("On", true).V("Priority", 0).V("Health", 0.994526).V("Value", 28373).Close()
            .Object().V("Slot", "Radar").V("Item", "int_sensors_size5_class2").V("On", true).V("Priority", 0).V("Health", 0.997511).V("Value", 79444).Close()
            .Object().V("Slot", "FuelTank").V("Item", "int_fueltank_size5_class3").V("On", true).V("Priority", 1).V("Health", 1.0).V("Value", 83090).Close()
            .Object().V("Slot", "Slot01_Size6").V("Item", "int_cargorack_size6_class1").V("On", true).V("Priority", 1).V("Health", 1.0).V("Value", 362591).Close()
            .Object().V("Slot", "Slot02_Size5").V("Item", "int_fuelscoop_size5_class5").V("On", true).V("Priority", 0).V("Health", 0.99527).V("Value", 9073694).Close()
            .Object().V("Slot", "Slot03_Size3").V("Item", "int_repairer_size3_class5").V("On", false).V("Priority", 0).V("Health", 0.986847).V("Value", 2624400).Close()
            .Object().V("Slot", "Slot05_Size3").V("Item", "int_shieldgenerator_size3_class5").V("On", true).V("Priority", 0).V("Health", 0.982536).V("Value", 507912).Close()
            .Object().V("Slot", "Slot06_Size2").V("Item", "int_buggybay_size2_class2").V("On", true).V("Priority", 0).V("Health", 0.993712).V("Value", 21600).Close()
            .Object().V("Slot", "Slot07_Size2").V("Item", "int_buggybay_size2_class2").V("On", false).V("Priority", 0).V("Health", 0.989521).V("Value", 21600).Close()
            .Object().V("Slot", "PlanetaryApproachSuite").V("Item", "int_planetapproachsuite_advanced").V("On", true).V("Priority", 1).V("Health", 1.0).Close()
            .Object().V("Slot", "VesselVoice").V("Item", "voicepack_verity").V("On", true).V("Priority", 1).V("Health", 1.0).Close()
            .Object().V("Slot", "Slot08_Size1").V("Item", "int_detailedsurfacescanner_tiny").V("On", true).V("Priority", 0).V("Health", 0.980316).V("Value", 212500).Close()
            .Object().V("Slot", "ShipCockpit").V("Item", "asp_cockpit").V("On", true).V("Priority", 1).V("Health", 0.986605).Close()
            .Object().V("Slot", "CargoHatch").V("Item", "modularcargobaydoor").V("On", false).V("Priority", 2).V("Health", 0.981758).Close()
            .Close()
            .Close();

            JObject jo = JObject.Parse(qj.Get());

            var sw = new Stopwatch();
            sw.Start();

            long avgtotal = 0;
            int trialstart = 3;
            int trials = 10;

            for (int trial = 0; trial < trials; trial++)
            {
                long totalticks = 0;
                for (int go = 0; go < 10000; go++)
                {
                    var tick1 = sw.ElapsedTicks;
                    JournalLoadout jl = new JournalLoadout(jo);
                    var tick2 = sw.ElapsedTicks;
                    totalticks += tick2 - tick1;
                }

                System.Diagnostics.Debug.WriteLine($"Trial {trial} time {(double)(totalticks) / Stopwatch.Frequency * 1000} ms");

                if (trial >= trialstart)
                    avgtotal += totalticks;

            }

            System.Diagnostics.Debug.WriteLine($"Avg Trial {trials - trialstart} time {(double)(avgtotal) / (trials - trialstart) / Stopwatch.Frequency * 1000} ms");



        }

        public class JournalLoadout
        {
            public JournalLoadout(JObject evt)
            {
                ShipFD = evt["Ship"].Str();
                Ship = ShipFD;
                ShipId = evt["ShipID"].ULong();
                ShipName = evt["ShipName"].Str();
                ShipIdent = evt["ShipIdent"].Str();
                HullValue = evt["HullValue"].LongNull();
                HullHealth = evt["HullHealth"].DoubleNull();
                if (HullHealth != null)
                    HullHealth *= 100.0;        // convert to 0-100
                ModulesValue = evt["ModulesValue"].LongNull();
                Rebuy = evt["Rebuy"].LongNull();
                Hot = evt["Hot"].BoolNull();    // 3.3
                UnladenMass = evt["UnladenMass"].DoubleNull(); // 3.4
                CargoCapacity = evt["CargoCapacity"].IntNull(); // 3.4
                MaxJumpRange = evt["MaxJumpRange"].DoubleNull(); // 3.4

                var fuelcap = evt["FuelCapacity"] as JObject; // 3.4

                if (fuelcap != null)
                {
                    MainFuelCapacity = fuelcap["Main"].DoubleNull();
                    ReserveFuelCapacity = fuelcap["Reserve"].DoubleNull();
                }


                JArray jmodules = (JArray)evt["Modules"];
                if (jmodules != null)       // paranoia
                {
                    ShipModules = new ShipModule[jmodules.Count];
                    int ii = 0;

                    foreach (JObject jo in jmodules)
                    {
                        EngineeringData engineering = null;

                        JObject jeng = (JObject)jo["Engineering"];
                        if (jeng != null)
                        {
                            engineering = new EngineeringData(jeng);
                            if (!engineering.IsValid)       // we get some bad engineering lines, if so, then remove the engineering
                            {
                                //System.Diagnostics.Debug.WriteLine($"Bad Engineering line loadout : {jo.ToString()}");
                                engineering = null;
                            }
                        }

                        ShipSlots.Slot slotfdname = ShipSlots.ToEnum(jo["Slot"].Str());
                        string itemfdname = jo["Item"].Str();

                        ShipModule module = new ShipModule(slotfdname.ToString(),
                                                            slotfdname,
                                                            itemfdname,
                                                            itemfdname,
                                                            jo["On"].BoolNull(),
                                                            jo["Priority"].IntNull(),
                                                            jo["AmmoInClip"].IntNull(),
                                                            jo["AmmoInHopper"].IntNull(),
                                                            jo["Health"].DoubleNull(),
                                                            jo["Value"].IntNull(),
                                                            null,  //power not received here
                                                            engineering);
                        ShipModules[ii++] = module;
                    }
                }
                else
                    ShipModules = new ShipModule[0];


            }

            public string Ship { get; set; }        // type, pretty name fer-de-lance
            public string ShipFD { get; set; }        // type,  fdname
            public ulong ShipId { get; set; }
            public string ShipName { get; set; } // : user-defined ship name
            public string ShipIdent { get; set; } //   user-defined ship ID string
            public long? HullValue { get; set; }   //3.0
            public double? HullHealth { get; set; }   //3.3, 1.0-0.0, multipled by 100.0
            public long? ModulesValue { get; set; }   //3.0
            public long? Rebuy { get; set; }   //3.0
            public bool? Hot { get; set; }   //3.3
            public double? UnladenMass { get; set; }   // 3.4
            public double? MainFuelCapacity { get; set; }   // 3.4
            public double? ReserveFuelCapacity { get; set; }   // 3.4
            public int? CargoCapacity { get; set; }   // 3.4
            public double? MaxJumpRange { get; set; }   // 3.4

            public ShipModule[] ShipModules;


        }

        [System.Diagnostics.DebuggerDisplay("{Slot} {Item} {LocalisedItem}")]
        public class ShipModule
        {
            public string Slot { get; private set; }        // never null       - english name
            public ShipSlots.Slot SlotFD { get; private set; }    // never null    
            public string Item { get; private set; }        // never null       - nice name, used to track, english
            public string ItemFD { get; private set; }      // never null     - FD normalised ID name
            public string LocalisedItem { get; set; }       // Modulex events only supply this. so it may be null if we have not seen one of them pass by with this Item name

            public bool? Enabled { get; private set; }      // Loadout events, may be null
            public int? Priority { get; private set; }      // 0..4 not 1..5
            public int? Health { get; private set; }        //0-100
            public long? Value { get; private set; }

            public int? AmmoClip { get; private set; }      // from loadout event
            public int? AmmoHopper { get; private set; }

            public double? Power { get; private set; }      // ONLY via Modules Info

            public EngineeringData Engineering { get; private set; }       // may be NULL if module is not engineered or unknown

            public ShipModule(string slotname, ShipSlots.Slot slotfdname, string itemname, string itemfdname,
                             bool? enabled, int? priority,
                             int? ammoclip, int? ammohopper,
                             double? health, long? value,
                             double? power,                  // only from Modules info
                             EngineeringData engineering)
            {
                Slot = slotname; SlotFD = slotfdname; Item = itemname; ItemFD = itemfdname; Enabled = enabled; Priority = priority;
                AmmoClip = ammoclip; AmmoHopper = ammohopper;
                if (health.HasValue)
                    Health = (int)(health * 100.0);
                Value = value;
                Power = power;
                Engineering = engineering;
            }


        }

        public class ShipSlots
        {
            public enum Slot
            {
                Unknown = 0,
                Armour,
                Bobble01,
                Bobble02,
                Bobble03,
                Bobble04,
                Bobble05,
                Bobble06,
                Bobble07,
                Bobble08,
                Bobble09,
                Bobble10,
                CargoHatch,
                CodexScanner,
                DataLinkScanner,
                DiscoveryScanner,
                Decal1,
                Decal2,
                Decal3,
                EngineColour,
                Federation_Fighter_Shield,
                FrameShiftDrive,
                FuelTank,
                GDN_Hybrid_Fighter_V1_Shield,
                GDN_Hybrid_Fighter_V2_Shield,
                GDN_Hybrid_Fighter_V3_Shield,
                HugeHardpoint1,
                HugeHardpoint2,
                Independent_Fighter_Shield,
                LargeHardpoint1,
                LargeHardpoint2,
                LargeHardpoint3,
                LargeHardpoint4,
                LifeSupport,
                MainEngines,
                MediumHardpoint1,
                MediumHardpoint2,
                MediumHardpoint3,
                MediumHardpoint4,
                MediumHardpoint5,
                Military01,
                Military02,
                Military03,
                PaintJob,
                PlanetaryApproachSuite,
                PowerDistributor,
                PowerPlant,
                Radar,
                ShieldGenerator,        // fighter
                ShipCockpit,
                ShipID0,
                ShipID1,
                ShipKitBumper,
                ShipKitSpoiler,
                ShipKitTail,
                ShipKitWings,
                ShipName0,
                ShipName1,
                Slot00_Size8,
                Slot01_Size2,
                Slot01_Size3,
                Slot01_Size4,
                Slot01_Size5,
                Slot01_Size6,
                Slot01_Size7,
                Slot01_Size8,
                Slot02_Size2,
                Slot02_Size3,
                Slot02_Size4,
                Slot02_Size5,
                Slot02_Size6,
                Slot02_Size7,
                Slot02_Size8,
                Slot03_Size1,
                Slot03_Size2,
                Slot03_Size3,
                Slot03_Size4,
                Slot03_Size5,
                Slot03_Size6,
                Slot03_Size7,
                Slot04_Size1,
                Slot04_Size2,
                Slot04_Size3,
                Slot04_Size4,
                Slot04_Size5,
                Slot04_Size6,
                Slot05_Size1,
                Slot05_Size2,
                Slot05_Size3,
                Slot05_Size4,
                Slot05_Size5,
                Slot05_Size6,
                Slot06_Size1,
                Slot06_Size2,
                Slot06_Size3,
                Slot06_Size4,
                Slot06_Size5,
                Slot07_Size1,
                Slot07_Size2,
                Slot07_Size3,
                Slot07_Size4,
                Slot07_Size5,
                Slot08_Size1,
                Slot08_Size2,
                Slot08_Size3,
                Slot08_Size4,
                Slot09_Size1,
                Slot09_Size2,
                Slot09_Size3,
                Slot09_Size4,
                Slot10_Size1,
                Slot10_Size2,
                Slot10_Size3,
                Slot10_Size4,
                Slot11_Size1,
                Slot11_Size2,
                Slot11_Size3,
                Slot12_Size1,
                Slot13_Size2,
                Slot14_Size1,
                SmallHardpoint1,
                SmallHardpoint2,
                SmallHardpoint3,
                SmallHardpoint4,
                SmallHardpoint5,        //type8
                SmallHardpoint6,        //type8
                StringLights,
                TinyHardpoint1,
                TinyHardpoint2,
                TinyHardpoint3,
                TinyHardpoint4,
                TinyHardpoint5,
                TinyHardpoint6,
                TinyHardpoint7,
                TinyHardpoint8,
                VesselVoice,
                WeaponColour,
                Turret,
                Turret2,        // reported by users
                SineWaveScanner,
                BuggyCargoHatch,
            }

            static Dictionary<string, Slot> QuickConvert;

            static ShipSlots()
            {
                QuickConvert = new Dictionary<string, Slot>();
                foreach (var v in Enum.GetValues(typeof(Slot)))
                    QuickConvert[v.ToString().ToLowerInvariant()] = (Slot)v;
            }
            public static Slot ToEnum(string fdname)
            {
                if (!fdname.HasChars())
                    return Slot.Unknown;

                if (QuickConvert.TryGetValue(fdname.ToLower(), out Slot s))
                    return s;

                if (Enum.TryParse(fdname, true, out Slot value))
                {
                    return value;
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine($"*** Slots unknown {fdname}");
                    return Slot.Unknown;
                }
            }

        }

        [System.Diagnostics.DebuggerDisplay("{Engineer} {BlueprintName} {Level} {ExperimentalEffect}")]
        public class EngineeringData
        {
            public string Engineer { get; set; }
            public string BlueprintName { get; set; }       // not case corrected - as inara gets it, best to leave its case.
            public string FriendlyBlueprintName { get; set; }
            public ulong EngineerID { get; set; }
            public ulong BlueprintID { get; set; }
            public int Level { get; set; }
            public double Quality { get; set; }
            public string ExperimentalEffect { get; set; }      // may be null or maybe empty (due to frontier) use HasChars()
            public string ExperimentalEffect_Localised { get; set; }    // may be null or maybe empty (due to frontier)
            public EngineeringModifiers[] Modifiers { get; set; }       // may be null

            public bool IsValid { get { return Level >= 1 && BlueprintName.HasChars(); } }

            public EngineeringData(JObject evt)
            {
                Engineer = evt["Engineer"].Str("Unknown");
                Level = evt["Level"].Int();

                if (evt.Contains("Blueprint"))     // old form
                {
                    BlueprintName = evt["Blueprint"].Str();
                }
                else
                {
                    EngineerID = evt["EngineerID"].ULong();     // NEW FORM after engineering changes in about 2018
                    BlueprintName = evt["BlueprintName"].Str();
                    BlueprintID = evt["BlueprintID"].ULong();
                    Quality = evt["Quality"].Double(0);

                    // EngineerCraft has it as Apply.. Loadout has just ExperimentalEffect.  Check both
                    ExperimentalEffect = evt.MultiStr(new string[] { "ExperimentalEffect", "ApplyExperimentalEffect" }, null);
                    if (ExperimentalEffect.HasChars())
                    {
                        string loc = evt["ExperimentalEffect_Localised"].StrNull();

                        // seen records with localised=experimental effect so protect that.
                        if (loc.EqualsIIC(ExperimentalEffect))
                        {
                          //  var recp = Recipes.FindRecipe(ExperimentalEffect);  // see if we have that recipe for backup name
                            //ExperimentalEffect_Localised = recp?.Name ?? ExperimentalEffect.SplitCapsWordFull();
                        }
                        else
                            ExperimentalEffect_Localised = loc;

                        //System.Diagnostics.Debug.WriteLine($"Exp effect {ExperimentalEffect} loc {loc} recp {recp?.Name} = {ExperimentalEffect_Localised}");
                    }

                    Modifiers = evt["Modifiers"]?.ToObject<EngineeringModifiers[]>(ignoretypeerrors: true);     // instances of Value being wrong type - ignore and continue

                    if (Modifiers != null)
                    {
                        foreach (EngineeringModifiers v in Modifiers)
                            v.FriendlyLabel = v.Label.Replace("_", " ").SplitCapsWord();
                    }
                    else
                    {

                    }
                }

                //FriendlyBlueprintName = BlueprintName.HasChars() ? Recipes.GetBetterNameForEngineeringRecipe(BlueprintName) : "??";       // some journal entries has empty blueprints
            }

        }
        [System.Diagnostics.DebuggerDisplay("{Label} {OriginalValue} -> {Value}")]
        public class EngineeringModifiers
        {
            public string Label { get; set; }
            public string FriendlyLabel { get; set; }
            public string ValueStr { get; set; }            // 3.02 if set, means ones further on do not apply. check first
            public string ValueStr_Localised { get; set; }
            public double Value { get; set; }               // may be 0
            public double OriginalValue { get; set; }
            public bool LessIsGood { get; set; }
        }

    }
}