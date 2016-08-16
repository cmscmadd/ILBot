using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ILBot
{
    [LuisModel("c17060ea-4265-443c-9071-fdbac0560b9b", "eccc3495509a41299b64e992dd1847ac")]
    [Serializable]
    public class IntelligentLayeringDialog : LuisDialog<object>
    {
        //private List<Layers> layers;
        //private static Dictionary<string, Layers> _basket = new Dictionary<string, Layers>();
        List<Shoes> _shoes = new List<Shoes>();

        public IntelligentLayeringDialog()
        {
            SeedData();
        }

        [LuisIntent("hello")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            //StateClient stateClient = activity.GetStateClient();


            var message = "Hello. I'm the Nike Intelligent Layering bot.  I can recommend layers for any condition and activity. ";


            await context.PostAsync(message);
            context.Wait(MessageReceived);

            //var message2 = twilio.SendMessage(
            //    "+15017250604", "+15558675309",
            //    "Hey Jenny! Good luck on the bar exam!",
            //    new string[] { "http://farm2.static.flickr.com/1075/1404618563_3ed9a44a3a.jpg" }
            //);
        }

        [LuisIntent("choose layer")]
        public async Task ChooseLayer(IDialogContext context, LuisResult result)
        {
            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>();
            
            EntityRecommendation dayOfWeek = new EntityRecommendation();
            result.TryFindEntity("builtin.datetime.date", out dayOfWeek);

            EntityRecommendation activityType = new EntityRecommendation();
            result.TryFindEntity("activitytype", out activityType);

            if (activityType != null)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://api.openweathermap.org/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("/data/2.5/forecast/daily?q=Portland&mode=json&units=metric&cnt=7&APPID=956ad244ac06c97de916c201e23c6113");
                    if (response.IsSuccessStatusCode)
                    {
                        Classes.WeatherResponse.Rootobject weatherResponse = await response.Content.ReadAsAsync<Classes.WeatherResponse.Rootobject>();
                        message.Text = string.Format("The weather in {0} is {1} on {2}, so you should definitely wear something with a swoosh on it when you go {3}.  Personally these sneakers are my favourite for {3} though. ![Nike](http://store.nike.com/us/en_us/pd/zoom-flyknit-streak-ultd-unisex-racing-shoe/pid-11056977/pgid-11140353)",
                           weatherResponse.city.name,
                           weatherResponse.list.FirstOrDefault().weather.FirstOrDefault().description,
                           dayOfWeek.Entity.First().ToString().ToUpper() + dayOfWeek.Entity.Substring(1),
                           activityType.Entity);

                        var rnd = new Random();
                        //var shoesData = _shoes.OrderBy(s => rnd.Next()).Take(1).ToList();
                        var shoesData = _shoes.Take(1).ToList();

                        foreach (var shoes in shoesData)
                        {
                            var attach = new Microsoft.Bot.Connector.Attachment()
                            {
                                ContentUrl = shoes.ContentUrl,
                                ContentType = shoes.ContentType,
                                ThumbnailUrl = shoes.ContentType,
                                Name = "LEBRON-XIII-ELITE-831923_170_A_PREM.jpg",
                            };

                            message.Attachments.Add(attach);
                        }

                        //message.ChannelData = "https://demo.twilio.com/owl.png";
                    }
                }
            }
            else
            {
                message.Text = $"I don't quite understand that, can you re-phrase it please?\" ";
            }

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("joke")]
        public async Task Joke(IDialogContext context, LuisResult result)
        {
            var message = $"What is E.T short for?  Because he's only got little legs.";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var message = $"I don't understand what you're asking of me, I can answer questions like \"What should I wear to go running on Sunday?.\" ";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        private void SeedData()
        {
            _shoes = new List<Shoes>()
            {
                new Shoes()
                {
                    Id = "00000001",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_COPY/LEBRON-XIII-ELITE-831923_170_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/lebron-xiii-elite-basketball-shoe/pid-10945535/pgid-11266227",
                    Text = "Built to the specifications of LeBron James, the LeBron XIII Elite Men's Basketball Shoe features a super-strong yet flexible upper lightweight midfoot reinforcement and enhanced cushioning for the aggressive postseason.",
                    Title = "LEBRON XIII ELITE",
                    Category = "Basketball",
                    Price = 165,
                    Color = "White"

                },
                new Shoes()
                {
                    Id = "00000002",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO/NIKE-FLYKNIT-LUNAREPIC-LOW-843764_401_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/lunarepic-low-flyknit-running-shoe/pid-11055906/pgid-11461313",
                    Text = "The Nike LunarEpic Low Flyknit Men's Running Shoe is lightweight and breathable with targeted cushioning for a soft, effortless sensation underfoot.",
                    Title = "NIKE LUNAREPIC LOW FLYKNIT",
                    Category = "Running",
                    Price = 130,
                    Color = "Blue"

                },
                new Shoes()
                {
                    Id = "00000003",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO_S/NIKE-ZOOM-VAPOR-95-TOUR-631458_802_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/nikecourt-zoom-vapor-9-5-tour-tennis-shoe/pid-10949021/pgid-11065461",
                    Text = "The NikeCourt Zoom Vapor 9.5 Tour Men's Tennis Shoe moulds to your foot for a locked-down fit, and it features super-responsive cushioning to meet the demands of your quickest turns and sprints.",
                    Title = "NIKECOURT ZOOM VAPOR 9.5 TOUR",
                    Category = "Tennis",
                    Price = 105,
                    Color = "Red"

                },
                new Shoes()
                {
                    Id = "00000004",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO_S/NIKE-FI-IMPACT-2-776111_600_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/fi-impact-2-golf-shoe/pid-10856029/pgid-109579781",
                    Text = "The Nike FI Impact 2 Men's Golf Shoe is designed with a stretchy yet supportive upper that adapts to the motion of your foot for a superior locked-in feel.",
                    Title = "NIKE FI IMPACT 2",
                    Category = "Golf",
                    Price = 100,
                    Color = "Red"

                },
                new Shoes()
                {
                    Id = "00000005",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO/CP-MAX-719912_007_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/air-max-sequent-running-shoe/pid-10944369/pgid-11261856",
                    Text = "The Nike Air Max Sequent Men's Running Shoe is made with a mesh upper and Max Air unit at the heel for lightweight breathability and soft, cushioned comfort.",
                    Title = "NIKE AIR MAX SEQUENT",
                    Category = "Running",
                    Price = 80,
                    Color = "Grey"
                },
                new Shoes()
                {
                    Id = "00000006",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO_S/Nike-Lunaracer-3-Mens-Running-Shoe-554675_804_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/lunaracer-3-running-shoe/pid-10944713/pgid-10956676",
                    Text = "The Nike Air Max Sequent Men's Running Shoe is made with a mesh upper and Max Air unit at the heel for lightweight breathability and soft, cushioned comfort.",
                    Title = "NIKE LUNARACER+ 3",
                    Category = "Running",
                    Price = 73.49,
                    Color = "Red"
                },
                new Shoes()
                {
                    Id = "00000007",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO_S/NATURAL-QUICKSTRIKE-827115_002_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/free-rn-distance-running-shoe/pid-10939948/pgid-11161865",
                    Text = "The Nike Free RN Distance Men's Running Shoe features Lunarlon cushioning with large hexagonal flex grooves for more natural motion with the soft comfort you need for longer runs.",
                    Title = "NIKE FREE RN DISTANCE",
                    Category = "Running",
                    Price = 73.49,
                    Color = "Black"
                },
                new Shoes()
                {
                    Id = "00000008",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO_S/NIKE-AIR-ZOOM-STRUCTURE-19-806580_001_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/air-zoom-structure-19-running-shoe/pid-10338564/pgid-11082944",
                    Text = "From the Flymesh upper to the triple-density foam midsole, the Nike Air Zoom Structure 19 Men's Running Shoe offers plenty of support and the response you need for a smooth, stable ride that feels ultra fast.",
                    Title = "NIKE AIR ZOOM STRUCTURE 19",
                    Category = "Running",
                    Price = 105,
                    Color = "Black"
                },
                new Shoes()
                {
                    Id = "00000009",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO/NIKE-FLYKNIT-LUNAREPIC-818676_401_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/lunarepic-flyknit-running-shoe/pid-11042244/pgid-11161727",
                    Text = "Built for the future of running and those who dare to lead it, the Men's Running Shoe delivers an impeccably smooth ride and a virtually unnoticeable, second-skin fit.",
                    Title = "NIKE LUNAREPIC FLYKNIT",
                    Category = "Running",
                    Price = 150,
                    Color = "Blue"
                },
                new Shoes()
                {
                    Id = "00000010",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO/NIKE-FLYKNIT-CHUKKA-819009_400_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/flyknit-chukka-golf-shoe/pid-10871705/pgid-11506672",
                    Text = "Dynamic and adaptive, the Nike Flyknit Chukka Men's Golf Shoe provides superior support, breathability and flexibility. The mid-top profile with a sock-like fit around the ankle keeps out debris and offers a bold look on the green.",
                    Title = "NIKE FLYKNIT CHUKKA",
                    Category = "Golf",
                    Price = 104.99,
                    Color = "Blue"
                },
                new Shoes()
                {
                    Id = "00000011",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO/Nike-Zoom-Victory-2-Unisex-Track-Spike-Mens-Sizing-555365_060_A.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/zoom-victory-2-distance-spike/pid-10020984/pgid-11454913",
                    Text = "Improving upon its predecessor with soft, durable Flywire cables and a single-layer mono mesh upper, the Nike Zoom Victory 2 Unisex Distance Spike is ideal for racing middle distances.",
                    Title = "NIKE ZOOM VICTORY 2",
                    Category = "Running",
                    Price = 110,
                    Color = "Black"
                }
                ,
                new Shoes()
                {
                    Id = "00000012",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO_S/NIKE-FREE-RN-FLYKNIT-831069_401_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/free-rn-flyknit-running-shoe/pid-10944243/pgid-11266209",
                    Text = "More cushioned than the Nike Free RN Motion Flyknit, the Nike Free RN Flyknit Men's Running features a newly designed midsole pattern that expands, flexes and contracts with your foot with every step.",
                    Title = "NIKE FREE RN FLYKNIT",
                    Category = "Running",
                    Price = 105,
                    Color = "Blue"
                }
                   ,
                new Shoes()
                {
                    Id = "00000013",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO_S/NIKE-FLYKNIT-CHUKKA-819009_400_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/flyknit-chukka-golf-shoe/pid-10871705/pgid-11161740",
                    Text = "More cushioned than the Nike Free RN Motion Flyknit, the Nike Free RN Flyknit Men's Running features a newly designed midsole pattern that expands, flexes and contracts with your foot with every step.",
                    Title = "NIKE FLYKNIT CHUKKA",
                    Category = "Golf",
                    Price = 104.99,
                    Color = "Blue"
                }
                ,
                new Shoes()
                {
                    Id = "00000014",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_HERO/NIKE-ZOOM-CAGE-2-QS-812934_101_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/nikecourt-zoom-cage-2-qs-tennis-shoe/pid-11140993/pgid-11264333",
                    Text = "The NikeCourt Zoom Cage 2 QS Unisex Tennis Shoe combines hardcore durability with lightweight support and stability.",
                    Title = "NIKECOURT ZOOM CAGE 2 QS",
                    Category = "Tennis",
                    Price = 95,
                    Color = "White"
                }
                ,
                new Shoes()
                {
                    Id = "00000015",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_COPY/Nike-Air-Vapor-Advantage-Mens-Tennis-Shoe-599359_810_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/nikecourt-air-vapor-advantage-tennis-shoe/pid-10949011/pgid-11469816",
                    Text = "The NikeCourt Air Vapor Advantage Men's Tennis Shoe offers enhanced comfort and long-lasting stability on the court with lightweight impact protection and a durable rubber outsole.",
                    Title = "NIKECOURT AIR VAPOR ADVANTAGE",
                    Category = "Tennis",
                    Price = 45.49,
                    Color = "Red"
                }
                ,
                new Shoes()
                {
                    Id = "00000016",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_COPY/NIKE-LUNAR-BALLISTEC-15-LG-812939_444_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/nikecourt-lunar-ballistec-1-5-legend-tennis-shoe/pid-10988179/pgid-11456621",
                    Text = "The NikeCourt Lunar Ballistec 1.5 Legend Men's Tennis Shoe is designed with an adaptive, lightweight fit for explosive play. ",
                    Title = "NIKECOURT LUNAR BALLISTEC 1.5 LEGEND",
                    Category = "Tennis",
                    Price = 150,
                    Color = "Blue"
                },
                new Shoes()
                {
                    Id = "00000017",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_COPY/NIKE-ZOOM-CAGE-2-EU-844960_800_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/zoom-cage-2-tennis-shoe/pid-11142097",
                    Text = "The Nike Zoom Cage 2 Men's Tennis Shoe helps you stay agile on the court. Durable side support protects during your fastest slides and responsive cushioning keeps your feet stable and comfortable with each step.",
                    Title = "NIKE ZOOM CAGE 2",
                    Category = "Tennis",
                    Price = 95,
                    Color = "Yellow"
                },
                new Shoes()
                {
                    Id = "00000018",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_COPY/NIKE-LUNAR-BALLISTEC-15-705285_484_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/nikecourt-lunar-ballistec-1-5-tennis-shoe/pid-11100989/pgid-11261810",
                    Text = "The NikeCourt Lunar Ballistec 1.5 Men's Tennis Shoe is designed with an adaptive, lightweight fit for explosive play. Nike Drag-On technology offers support and protection where you need it most.",
                    Title = "NIKECOURT LUNAR BALLISTEC 1.5",
                    Category = "Tennis",
                    Price = 130,
                    Color = "Blue"
                },
                new Shoes()
                {
                    Id = "00000019",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_COPY/NIKE-LUNAR-COMMAND-704427_001_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/lunar-command-golf-shoe/pid-10191479/pgid-10295621",
                    Text = "The Nike Lunar Command Men's Golf Shoe is made with a lightweight microfibre leather upper and Flywire technology for comfort and adaptive support from your first drive to your final putt.",
                    Title = "NIKE LUNAR COMMAND",
                    Category = "Golf",
                    Price = 85,
                    Color = "Black"
                },
                new Shoes()
                {
                    Id = "00000020",
                    ContentType = "image/jpg",
                    ContentUrl = "http://images.nike.com/is/image/DotCom/PDP_COPY/NIKE-GOLF-LUNAR-FORCE-1-818726_400_A_PREM.jpg",
                    Link = "http://store.nike.com/gb/en_gb/pd/lunar-force-1-golf-shoe/pid-11051837/pgid-11161729",
                    Text = "Combining the silhouette of a classic Nike Air Force 1 with a design made for golf, the Nike Lunar Force 1 Men's golf shoe brings an iconic look to the course while giving you a stable, comfortable stride from tee to green.",
                    Title = "NIKE LUNAR FORCE 1",
                    Category = "Golf",
                    Price = 110,
                    Color = "Blue"
                }
            };
        }
    }

}
