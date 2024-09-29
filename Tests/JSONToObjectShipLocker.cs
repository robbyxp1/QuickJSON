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
        public void JSONToObjectShipLocker()
        {
            JSONFormatter qj = new JSONFormatter();

            qj.Object().V("event", "ShipLocker")
            .Array("Items")
            .Object().V("Name", "chemicalsample").V("Name_Localised", "Chemical Sample").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "gmeds").V("Name_Localised", "G-Meds").V("OwnerID", 0).V("Count", 3).Close()
            .Object().V("Name", "healthmonitor").V("Name_Localised", "Health Monitor").V("OwnerID", 145391).V("Count", 2).Close()
            .Object().V("Name", "healthmonitor").V("Name_Localised", "Health Monitor").V("OwnerID", 0).V("Count", 29).Close()
            .Object().V("Name", "insight").V("OwnerID", 0).V("Count", 18).Close()
            .Object().V("Name", "insightdatabank").V("Name_Localised", "Insight Data Bank").V("OwnerID", 0).V("Count", 3).Close()
            .Object().V("Name", "personalcomputer").V("Name_Localised", "Personal Computer").V("OwnerID", 0).V("Count", 2).Close()
            .Object().V("Name", "compactlibrary").V("Name_Localised", "Compact Library").V("OwnerID", 0).V("Count", 39).Close()
            .Object().V("Name", "hush").V("OwnerID", 0).V("Count", 2).Close()
            .Object().V("Name", "infinity").V("OwnerID", 0).V("Count", 21).Close()
            .Object().V("Name", "insightentertainmentsuite").V("Name_Localised", "Insight Entertainment Suite").V("OwnerID", 0).V("Count", 18).Close()
            .Object().V("Name", "lazarus").V("OwnerID", 0).V("Count", 8).Close()
            .Object().V("Name", "nutritionalconcentrate").V("Name_Localised", "Nutritional Concentrate").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "personaldocuments").V("Name_Localised", "Personal Documents").V("OwnerID", 0).V("Count", 2).Close()
            .Object().V("Name", "push").V("OwnerID", 0).V("Count", 3).Close()
            .Object().V("Name", "syntheticpathogen").V("Name_Localised", "Synthetic Pathogen").V("OwnerID", 0).V("Count", 4).Close()
            .Object().V("Name", "universaltranslator").V("Name_Localised", "Universal Translator").V("OwnerID", 0).V("Count", 11).Close()
            .Object().V("Name", "weaponschematic").V("Name_Localised", "Weapon Schematic").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "agriculturalprocesssample").V("Name_Localised", "Agricultural Process Sample").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "compressionliquefiedgas").V("Name_Localised", "Compression-Liquefied Gas").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "degradedpowerregulator").V("Name_Localised", "Degraded Power Regulator").V("OwnerID", 0).V("Count", 22).Close()
            .Object().V("Name", "largecapacitypowerregulator").V("Name_Localised", "Power Regulator").V("OwnerID", 0).V("Count", 4).Close()
            .Close()
            .Array("Components")
            .Object().V("Name", "circuitboard").V("Name_Localised", "Circuit Board").V("OwnerID", 0).V("MissionID", 787859621).V("Count", 1).Close()
            .Object().V("Name", "graphene").V("OwnerID", 0).V("Count", 9).Close()
            .Object().V("Name", "circuitboard").V("Name_Localised", "Circuit Board").V("OwnerID", 0).V("Count", 9).Close()
            .Object().V("Name", "circuitswitch").V("Name_Localised", "Circuit Switch").V("OwnerID", 0).V("Count", 33).Close()
            .Object().V("Name", "electricalfuse").V("Name_Localised", "Electrical Fuse").V("OwnerID", 0).V("Count", 8).Close()
            .Object().V("Name", "electricalwiring").V("Name_Localised", "Electrical Wiring").V("OwnerID", 0).V("Count", 7).Close()
            .Object().V("Name", "encryptedmemorychip").V("Name_Localised", "Encrypted Memory Chip").V("OwnerID", 0).V("Count", 12).Close()
            .Object().V("Name", "epoxyadhesive").V("Name_Localised", "Epoxy Adhesive").V("OwnerID", 0).V("Count", 16).Close()
            .Object().V("Name", "memorychip").V("Name_Localised", "Memory Chip").V("OwnerID", 0).V("Count", 12).Close()
            .Object().V("Name", "metalcoil").V("Name_Localised", "Metal Coil").V("OwnerID", 0).V("Count", 16).Close()
            .Object().V("Name", "microsupercapacitor").V("Name_Localised", "Micro Supercapacitor").V("OwnerID", 0).V("Count", 11).Close()
            .Object().V("Name", "microtransformer").V("Name_Localised", "Micro Transformer").V("OwnerID", 0).V("Count", 10).Close()
            .Object().V("Name", "motor").V("OwnerID", 0).V("Count", 11).Close()
            .Object().V("Name", "opticalfibre").V("Name_Localised", "Optical Fibre").V("OwnerID", 0).V("Count", 19).Close()
            .Object().V("Name", "opticallens").V("Name_Localised", "Optical Lens").V("OwnerID", 0).V("Count", 7).Close()
            .Object().V("Name", "scrambler").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "transmitter").V("OwnerID", 0).V("Count", 2).Close()
            .Object().V("Name", "electromagnet").V("OwnerID", 0).V("Count", 10).Close()
            .Object().V("Name", "oxygenicbacteria").V("Name_Localised", "Oxygenic Bacteria").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "microelectrode").V("OwnerID", 0).V("Count", 22).Close()
            .Object().V("Name", "ionbattery").V("Name_Localised", "Ion Battery").V("OwnerID", 0).V("Count", 17).Close()
            .Close()
            .Array("Consumables")
            .Object().V("Name", "healthpack").V("Name_Localised", "Medkit").V("OwnerID", 0).V("Count", 100).Close()
            .Object().V("Name", "energycell").V("Name_Localised", "Energy Cell").V("OwnerID", 0).V("Count", 100).Close()
            .Object().V("Name", "amm_grenade_emp").V("Name_Localised", "Shield Disruptor").V("OwnerID", 0).V("Count", 100).Close()
            .Object().V("Name", "amm_grenade_frag").V("Name_Localised", "Frag Grenade").V("OwnerID", 0).V("Count", 100).Close()
            .Object().V("Name", "amm_grenade_shield").V("Name_Localised", "Shield Projector").V("OwnerID", 0).V("Count", 100).Close()
            .Object().V("Name", "bypass").V("Name_Localised", "E-Breach").V("OwnerID", 0).V("Count", 100).Close()
            .Close()
            .Array("Data")
            .Object().V("Name", "internalcorrespondence").V("Name_Localised", "Internal Correspondence").V("OwnerID", 0).V("Count", 5).Close()
            .Object().V("Name", "biometricdata").V("Name_Localised", "Biometric Data").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "nocdata").V("Name_Localised", "NOC Data").V("OwnerID", 0).V("Count", 5).Close()
            .Object().V("Name", "airqualityreports").V("Name_Localised", "Air Quality Reports").V("OwnerID", 145391).V("Count", 4).Close()
            .Object().V("Name", "airqualityreports").V("Name_Localised", "Air Quality Reports").V("OwnerID", 0).V("Count", 5).Close()
            .Object().V("Name", "atmosphericdata").V("Name_Localised", "Atmospheric Data").V("OwnerID", 0).V("Count", 3).Close()
            .Object().V("Name", "blacklistdata").V("Name_Localised", "Blacklist Data").V("OwnerID", 0).V("Count", 4).Close()
            .Object().V("Name", "censusdata").V("Name_Localised", "Census Data").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "chemicalexperimentdata").V("Name_Localised", "Chemical Experiment Data").V("OwnerID", 0).V("Count", 2).Close()
            .Object().V("Name", "combattrainingmaterial").V("Name_Localised", "Combat Training Material").V("OwnerID", 145391).V("Count", 1).Close()
            .Object().V("Name", "factionnews").V("Name_Localised", "Faction News").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "genesequencingdata").V("Name_Localised", "Gene Sequencing Data").V("OwnerID", 0).V("Count", 2).Close()
            .Object().V("Name", "maintenancelogs").V("Name_Localised", "Maintenance Logs").V("OwnerID", 0).V("Count", 5).Close()
            .Object().V("Name", "maintenancelogs").V("Name_Localised", "Maintenance Logs").V("OwnerID", 145391).V("Count", 9).Close()
            .Object().V("Name", "manufacturinginstructions").V("Name_Localised", "Manufacturing Instructions").V("OwnerID", 0).V("Count", 4).Close()
            .Object().V("Name", "medicalrecords").V("Name_Localised", "Medical Records").V("OwnerID", 145391).V("Count", 1).Close()
            .Object().V("Name", "medicalrecords").V("Name_Localised", "Medical Records").V("OwnerID", 0).V("Count", 4).Close()
            .Object().V("Name", "mineralsurvey").V("Name_Localised", "Mineral Survey").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "mineralsurvey").V("Name_Localised", "Mineral Survey").V("OwnerID", 145391).V("Count", 2).Close()
            .Object().V("Name", "mininganalytics").V("Name_Localised", "Mining Analytics").V("OwnerID", 0).V("Count", 7).Close()
            .Object().V("Name", "operationalmanual").V("Name_Localised", "Operational Manual").V("OwnerID", 0).V("Count", 2).Close()
            .Object().V("Name", "patrolroutes").V("Name_Localised", "Patrol Routes").V("OwnerID", 145391).V("Count", 2).Close()
            .Object().V("Name", "propaganda").V("OwnerID", 0).V("Count", 2).Close()
            .Object().V("Name", "radioactivitydata").V("Name_Localised", "Radioactivity Data").V("OwnerID", 0).V("Count", 4).Close()
            .Object().V("Name", "reactoroutputreview").V("Name_Localised", "Reactor Output Review").V("OwnerID", 145391).V("Count", 5).Close()
            .Object().V("Name", "recyclinglogs").V("Name_Localised", "Recycling Logs").V("OwnerID", 145391).V("Count", 2).Close()
            .Object().V("Name", "riskassessments").V("Name_Localised", "Risk Assessments").V("OwnerID", 145391).V("Count", 7).Close()
            .Object().V("Name", "securityexpenses").V("Name_Localised", "Security Expenses").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "securityexpenses").V("Name_Localised", "Security Expenses").V("OwnerID", 145391).V("Count", 2).Close()
            .Object().V("Name", "surveilleancelogs").V("Name_Localised", "Surveillance Logs").V("OwnerID", 0).V("Count", 3).Close()
            .Object().V("Name", "topographicalsurveys").V("Name_Localised", "Topographical Surveys").V("OwnerID", 0).V("Count", 1).Close()
            .Object().V("Name", "troopdeploymentrecords").V("Name_Localised", "Troop Deployment Records").V("OwnerID", 145391).V("Count", 1).Close()
            .Object().V("Name", "weaponinventory").V("Name_Localised", "Weapon Inventory").V("OwnerID", 145391).V("Count", 3).Close()
            .Object().V("Name", "weaponinventory").V("Name_Localised", "Weapon Inventory").V("OwnerID", 0).V("Count", 5).Close()
            .Object().V("Name", "weapontestdata").V("Name_Localised", "Weapon Test Data").V("OwnerID", 145391).V("Count", 4).Close()
            .Object().V("Name", "pharmaceuticalpatents").V("Name_Localised", "Pharmaceutical Patents").V("OwnerID", 0).V("Count", 2).Close()
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
                for (int go = 0; go < 1000; go++)
                {
                    var tick1 = sw.ElapsedTicks;
                    JournalMicroResourceState jl = new JournalMicroResourceState(jo);
                    var tick2 = sw.ElapsedTicks;
                    totalticks += tick2 - tick1;
                }

                System.Diagnostics.Debug.WriteLine($"Trial {trial} time {(double)(totalticks) / Stopwatch.Frequency * 1000} ms");


                if (trial >= trialstart)
                    avgtotal += totalticks;
            }

            System.Diagnostics.Debug.WriteLine($"Avg Trial {trials-trialstart} time {(double)(avgtotal) / (trials - trialstart) / Stopwatch.Frequency * 1000} ms");



        }


        public class JournalMicroResourceState 
        {
            public JournalMicroResourceState(JObject evt) 
            {
                Rescan(evt);
            }

            public void Rescan(JObject evt)
            {
                // these collect Name, Name_Localised, MissionID, OwnerID, Count

                Items = evt["Items"]?.ToObjectQ<MicroResource[]>()?.ToArray();
                MicroResource.Normalise(Items, "Items");
                Components = evt["Components"]?.ToObjectQ<MicroResource[]>()?.ToArray();
                MicroResource.Normalise(Components, "Components");
                Consumables = evt["Consumables"]?.ToObjectQ<MicroResource[]>()?.ToArray();
                MicroResource.Normalise(Consumables, "Consumables");
                Data = evt["Data"]?.ToObjectQ<MicroResource[]>()?.ToArray();
                MicroResource.Normalise(Data, "Data");
            }


            public MicroResource[] Items { get; set; }
            public MicroResource[] Components { get; set; }
            public MicroResource[] Consumables { get; set; }
            public MicroResource[] Data { get; set; }

        }


        public class MicroResource
        {
            public const int ShipLocker = 0;                        // index into MCMRList for types
            public const int BackPack = 1;

            public string Name { get; set; }                        // JSON, normalised to lower case, All
            public string Name_Localised { get; set; }              // JSON, All

            public string FriendlyName { get; set; }                // computed

            public ulong OwnerID { get; set; }                      // JSON, ShipLockerMaterials          |, CollectItems, DropItems
            public ulong MissionID { get; set; }                    // JSON, ShipLockerMaterials          |, DropItems, may be -1, invalid

            public int Count { get; set; }                          // JSON, ShipLockerMaterials, BuyMicroResource, SellMicroResource, TradeMicroResources

            [JsonNameAttribute("Type", "Category")]                 // for some crazy reason, they use type someplaces, category others. Use json name to allow both
            public string Category { get; set; }                    // JSON, BuyMicroResources, SellMicroResource, TradeMicroResources, TransferMicroResources. These call it type: BackPackChange, UseConsumable, CollectItems, DropItems   

            public void Normalise(string cat)
            {
                if (Name.HasChars())
                {
                    //Name_Localised = CheckLocalisation(Name_Localised, Name);
                    //Name = JournalFieldNaming.FDNameTranslation(Name);      // this lower cases the name
                    //FriendlyName = MaterialCommodityMicroResourceType.GetTranslatedNameByFDName(Name);      // normalises to lower case  
                    if (cat != null)
                        Category = cat;
                }
                else
                {
                    Name = Name_Localised = FriendlyName = "Missing Microresource Name - report";
                    Category = "ERROR";
                    System.Diagnostics.Trace.WriteLine("Microresource journal without Name detected");
                }
            }

            static public void Normalise(MicroResource[] a, string cat)
            {
                foreach (MicroResource m in a.EmptyIfNull())
                    m.Normalise(cat);
            }

        }


    }
}