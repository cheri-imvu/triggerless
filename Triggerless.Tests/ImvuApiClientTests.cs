using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Models;
using Triggerless.Services.Server;

namespace Triggerless.Tests
{
    [TestFixture]
    public class ImvuApiClientTests
    {
        private const long USER_ID = 70587910; //oP0PPYo
        private const long PRoDUCT_ID = 47935570; //thb timbs
        private ImvuApiClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new ImvuApiClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
        }

        [Test]
        public async Task UserSingle()
        {
            var user = await _client.GetUser(USER_ID);
            Assert.IsNotNull(user, "null");
            Assert.That(user.AvatarName.ToLower().Contains("p0ppy"), "not p0ppy");
        } 

        [Test]
        public async Task ProductSingle()
        {
            var product = await _client.GetProduct(PRoDUCT_ID);
            Assert.IsNotNull(product, "null");
            Assert.That(product.Name.ToLower().Contains("timb"), "not timb");

        }

        [Test]
        public async Task ProductMultiple()
        {
            var pids = new long[] { 10599277, 52874386, 52874314, 52797258, 52459235, 52439278, 52439111 };
            var products = await _client.GetProducts(pids);
            Assert.IsNotNull(products);
            Assert.AreEqual(pids.Length, products.Products.Count());
            Assert.AreEqual(pids.Length, products.Products.Count(p => p.Status == "success"));
        }

        [Test]
        public async Task ProductSingleHidden()
        {
            long hiddenProductID = 41967L; // Ankh Collar
            var product = await _client.GetProduct(hiddenProductID);
            Assert.IsNotNull(product);
            Assert.IsNotNull(product.Name);
            Assert.IsNotNull(product.CreatorName);
            Assert.IsNotNull(product.ProductImage);
            Assert.That(product.CreatorId > 0);
        }

        [Test]
        public async Task GetAvatarCardById()
        {
            var json = await _client.GetAvatarCardJson($"25522141");
            Assert.IsNotNull(json);
            Assert.IsNotNull(JObject.Parse(json));
            Console.WriteLine(json);
        }

        [Test]
        public async Task GetAvatarCardByName()
        {
            var json = await _client.GetAvatarCardJson($"DJSher");
            Assert.IsNotNull(json);
            Assert.IsNotNull(JObject.Parse(json));
            Console.WriteLine(json);
        }

        [Test]
        public async Task GetAvatarCardBadId()
        {
            var json = await _client.GetAvatarCardJson($"255221410000");
            Assert.IsNotNull(json);
            Assert.IsNotNull(JObject.Parse(json));
            object o = JsonConvert.DeserializeObject(json);

            Assert.That(condition: o.GetType()
                .GetProperties()
                .Any(prop => prop.Name == "error"));

            Assert.That(condition: o.GetType()
                .GetProperties()
                .Where(prop => prop.Name == "error")
                .First()
                .GetValue(o)
                .ToString() == "No avatar was specified.");

            Console.WriteLine(json);

        }

        [Test]
        public async Task GetAvatarCardBadName()
        {
            var json = await _client.GetAvatarCardJson($"xxPresidentHillaryClintonxx");
            Assert.IsNotNull(json);
            Assert.IsNotNull(JObject.Parse(json));
            object o = JsonConvert.DeserializeObject(json);

            Assert.That(condition: o.GetType()
                .GetProperties()
                .Any(prop => prop.Name == "error"));

            Assert.That(condition: o.GetType()
                .GetProperties()
                .Where(prop => prop.Name == "error")
                .First()
                .GetValue(o)
                .ToString() == "No avatar was specified.");

            Console.WriteLine(json);

        }

        [Test]
        public async Task GetConversationJson()
        {
            var json = await _client.GetConversationsJson();
            Console.WriteLine(json);
        }

        [Test]
        public async Task GetConvoResponse()
        {
            var resp = await _client.ConversationResponse();
            Assert.IsTrue(resp != null);
            Assert.That(resp.Conversations.Count > 10);
            Console.WriteLine($"{resp.Conversations.Count} conversations");

        }

        [Test]
        public async Task GetOutfits()
        {
            var req = new ExposeOutfitsRequest();
            req.Entries = GetExposeOutfitsRequest().Entries.Where(a => a.AvatarId != -1).Skip(40).Take(20).ToArray();
            var response = await _client.GetOutfits(req);
            Assert.IsTrue(response != null);
            Assert.IsTrue(response.Entries.Length == req.Entries.Length, $"Requested {req.Entries.Length}, Responded {response.Entries.Length}");
            Console.WriteLine($"Nice, {response.Entries.Length}");
            var lastEntry = response.Entries[response.Entries.Length - 1];
            Assert.IsTrue(lastEntry.Products.Length > 0, $"Products missing, naked avatar {lastEntry.User.AvatarName}");
            Console.WriteLine($"Outfit for {lastEntry.User.AvatarName}");
            foreach (var product in lastEntry.Products)
            {
                Console.WriteLine($"\t{product.Name} by {product.CreatorName}");
            }

            var prevName = "";
            var comparer = CaseInsensitiveComparer.Default;
            Console.WriteLine("Avatars:");
            foreach (var respEntry in response.Entries) {
                var modName = respEntry.User.AvatarName.Replace("Guest_", "");
                var comparison = comparer.Compare(prevName, modName);
                if (comparison < 0)
                {
                    prevName = modName;
                    Console.WriteLine($"\t{prevName}");
                }
                else
                {
                    Assert.Fail("The avatar names are not in alphabetical order");
                }
            }
        }

        [Test]
        public void ParseUrlToOutfitRequest()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Triggerless.Tests.long-url.txt"))
            {
                byte[] bytes = new byte[stream.Length];
                var bytesRead = stream.Read(bytes, 0, (int)stream.Length);
                string url = Encoding.UTF8.GetString(bytes);
                var startTime = DateTimeOffset.Now;
                var result = ImvuApiClient.RequestFromUrl(url);
                //Console.WriteLine($"It took {(DateTimeOffset.Now - startTime).TotalMilliseconds} ms");
                Assert.IsTrue(result.Entries.Length == 51, $"Entries, expected 51, found {result.Entries.Length}");
                Console.Write(JsonConvert.SerializeObject(result));
            }
        }

        private ExposeOutfitsRequest GetExposeOutfitsRequest()
        {
            var entries = new List<ExposeOutfitsRequestEntry>();
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 34846004,
                ProductIds = new long[] { 15531348, 20383120, 28409784, 33306159, 39072871, 39262679, 41514228, 42437560, 44779587, 47504535, 48346504, 48358218, 50810194, 55557544, 55717857, 56145673, 57718886, 59745854, 62104928 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 172729744,
                ProductIds = new long[] { 5192100, 11849709, 15233670, 15693995, 16070338, 27276668, 31495397, 35749912, 36737296, 37396667, 38200038, 40547023, 42112147, 42216576, 46639322, 52821015 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 275690530,
                ProductIds = new long[] { 80, 13683313, 19610450, 40094777, 45688842, 48974108, 50260822, 51971907, 52505438, 52963001, 53880624, 55413677, 61784310 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 143684716,
                ProductIds = new long[] { 191, 8702073, 20063355, 25577127, 25812667, 26664085, 28643430, 30163695, 30844762, 32901294, 33362689, 43452918, 53956761 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 125403219,
                ProductIds = new long[] { 2774055, 6897017, 12652094, 15531453, 21503240, 26813733, 30936279, 33244916, 38790404, 39440108, 39496248, 43025075, 43557106, 44790313, 45282669, 46307662, 47727533, 49865628, 52791621, 53909361, 58449536 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 68581659,
                ProductIds = new long[] { 80, 814734, 3172679, 7900985, 12134163, 13683129, 33453024, 37323115, 43059815, 45388810, 52127336, 54079559, 54218631, 54406047, 54577591, 55119105, 57803719, 60926840, 62012186 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 285951369,
                ProductIds = new long[] { 12182900, 13683303, 13831026, 22278658, 34947255, 34992854, 36235271, 36897701, 38660422, 42265085, 44096237, 48383448, 49259114, 51264214, 51925782, 53105964, 53447178, 53698021, 54596751, 56488109 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 105173690,
                ProductIds = new long[] { 1194500, 4802877, 10519529, 12682569, 13324612, 15238019, 15466770, 19248941, 19489929, 23090601, 23191481, 26371178, 27931594, 27932012, 27932057, 28180416, 31498980, 33750919, 38706297, 43569682, 45308991, 45309075, 47087615, 48568573, 51272347, 57773099 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 352809400,
                ProductIds = new long[] { 13759128, 19465733, 27652342, 32034675, 45358429, 52728559, 53671573, 55280879, 55945565, 59911395, 60677244, 60926840, 60974214, 61434300 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 23609202,
                ProductIds = new long[] { 80, 1336037, 4898872, 5020618, 6161034, 7671705, 19442649, 25624803, 25980204, 29173285, 35481675, 37265131, 37323143, 38611403, 38690513, 39216328, 40399651, 41897357, 42174059, 45784944, 50967369, 53453938, 53880624, 54220066, 60308243, 61283283 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 216126977,
                ProductIds = new long[] { 356320, 16070361, 38463375, 42437413, 47362255, 51069360, 52875403, 54446327, 55133727, 55304063, 56873130, 58116834, 58116853, 58174738, 58175391, 58393199, 60998899, 60999552, 61000823, 61281559, 61892785, 61969221 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 131332766,
                ProductIds = new long[] { 191, 4714921, 9112925, 9260897, 11745247, 13926389, 14067114, 15366691, 15531431, 18667941, 20203262, 21164940, 21503232, 22433371, 24624349, 27376710, 31896704, 34311417, 37613149, 54440396 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 27074071,
                ProductIds = new long[] { 16070338, 22275359, 26105098, 33061817, 36123016, 36451213, 37939705, 38914114, 49346123, 53519475, 57054470, 57389512, 58115036, 58406615, 58775721, 59132707, 59787771, 60243468, 61216004, 61637845 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 139874493,
                ProductIds = new long[] { 191, 12120618, 13831026, 19818827, 26896915, 28643430, 32653203, 40551479, 44284451, 44798744, 45446348, 46404206, 47812919, 47835581, 47836979 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 206505880,
                ProductIds = new long[] { 8702728, 28329882, 28409784, 28946096, 36748761, 41329818, 41419975, 43830236, 44367901, 45908088, 49244591, 49945439, 51988620, 52262308, 53379040, 59056664 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 48624290,
                ProductIds = new long[] { 15531524, 21502905, 22275280, 33151665, 41474376, 42609501, 45032889, 47606423, 49453512, 50026657, 53035160, 53710827, 55825908, 59569654, 61393246, 61491206 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 87112305,
                ProductIds = new long[] { 21560762, 23090601, 34724125, 37048173, 38472583, 39037704, 41513982, 45403863, 47404533, 48084536, 48205823, 48403198, 48560714, 49501718, 49691113, 49953539 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 106757390,
                ProductIds = new long[] { 6976831, 12134163, 14310230, 16070338, 19848536, 20459703, 21662180, 21950440, 22155363, 22285473, 25979418, 28111707, 28657694, 28921236, 30836023, 32109390, 32361877, 37009609, 37055395, 45671973, 46207461, 46765502, 47273623, 48768521, 50608241, 51270308, 52058298, 53880624, 55316103, 58449485, 60651567, 62001135 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 194575138,
                ProductIds = new long[] { 13683313, 15958580, 22728543, 28329882, 36351164, 38021820, 41405780, 41593431, 42830763, 45228529, 48937752, 49679966, 49722570, 52960267, 57249219, 59217230, 60401174, 61533744, 62089059 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 183300492,
                ProductIds = new long[] { 13683015, 13831600, 15531441, 17455252, 23962235, 29092523, 30664388, 32051421, 32793072, 42378016, 46388826, 51339019, 53841504, 58483797 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 195418323,
                ProductIds = new long[] { 80, 9214974, 19660528, 21115929, 36704480, 38840879, 40921447, 44367901, 44888094, 46932591, 49151266, 49945439, 49955250, 53807601 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 193051653,
                ProductIds = new long[] { 80, 13683207, 29931497, 30396585, 37107288, 38068502, 38914120, 41303062, 43769507, 44708954, 53756757 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 46929799,
                ProductIds = new long[] { 13831517, 15531453, 17112994, 20530221, 20724459, 21503288, 24011506, 27871983, 28731925, 28731939, 30149379, 30611148, 32778826, 32902177, 35947090, 41341150, 42493839, 54158507 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 132050172,
                ProductIds = new long[] { 267582, 12134163, 13683313, 15482548, 15701517, 23504253, 24019424, 24363231, 27132487, 27132562, 28390239, 35212336, 41275823, 44254979, 47760485, 49491755, 49612004, 51202654, 51851277, 51941829, 53464755, 53562484, 54212295, 55412161, 56766311, 56935890, 57194472, 57456255, 57515217, 57801973, 59614074, 59680748 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 48093496,
                ProductIds = new long[] { 191, 12182900, 22278628, 22852510, 23274147, 30844762, 34212972, 37264196, 38840633, 39216012, 43798383, 44307202, 52806117, 53251129, 54837144 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 177632643,
                ProductIds = new long[] { 11721200, 15531366, 19486906, 26971411, 28174524, 30955914, 32174972, 33326320, 40315766, 42554884, 42818820, 43542749, 43688550, 43805920, 44026015, 44884836, 47658214, 47804291, 50468460, 50865568, 50879303, 51165776, 58925454 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 193632656,
                ProductIds = new long[] { 80, 4032279, 8895760, 13759128, 21926876, 24917132, 33992471, 34512216, 38215914, 38658460, 38976834, 40702180, 42432568, 44372535, 44768143, 45808130, 46683384, 46694208, 47504725, 49679954, 50551602, 56629703 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 280808422,
                ProductIds = new long[] { 191, 13759128, 28574153, 29988570, 30163748, 30844762, 32007158, 32793072, 37892797, 41321781, 46517854, 48359753, 49259114, 49806864, 50534458, 50851958, 53696166, 56577273, 56912694, 58166671, 61916315 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 142820132,
                ProductIds = new long[] { 191, 5059886, 5677226, 6021881, 20739036, 27131334, 29176238, 30785576, 31227591, 35711631, 38844982, 39527122, 53619556 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 139459094,
                ProductIds = new long[] { 80, 8379095, 22036888, 22376960, 26048180, 26928647, 34124072, 36257107, 38026809, 39828438, 40623976, 42420717, 45661269, 46562384, 50126472, 52492250, 52593245, 54433042, 55278869, 55309886, 55955575, 59190082, 59555902, 59569741, 60849811, 62236296 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 349721740,
                ProductIds = new long[] { 31319979, 32352471, 40096298, 40866776, 42772529, 43821763, 43851250, 43851298, 43851409, 57561902, 58573339, 61641846 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 94163176,
                ProductIds = new long[] { 191, 6010716, 12120618, 28487583, 28943527, 36487727, 40942696, 41341150, 43851409, 44412021, 49433646, 55624956 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 285218032,
                ProductIds = new long[] { 11165541, 12198477, 16070361, 24150123, 24164867, 25310925, 33753039, 43553536, 46139477, 49364837, 49664088, 50291576, 51229071, 51981192, 53808705, 54939135, 56179878, 56532880, 57704980, 58114661, 58116731, 58116834, 58116853, 58117061, 58117688, 59166146, 61687072 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 277919557,
                ProductIds = new long[] { 191, 8676692, 11468262, 12153804, 15531453, 16063360, 19819882, 23750851, 30297934, 35023089, 38837733, 40528511, 45301239, 50639120, 53365598, 58059865, 58124296, 58909766 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 161893215,
                ProductIds = new long[] { 191, 4431157, 16237151, 22852529, 22943517, 28745151, 28943527, 29644788, 31365992, 31744895, 32650775, 32839856, 34374607, 34823497, 35511901, 54183945 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 194920439,
                ProductIds = new long[] { 444337, 13759128, 14168558, 22278635, 28329882, 33753039, 33776465, 36692514, 36883339, 39319368, 39319406, 42399131, 44367901, 49033965, 49084871, 49088596, 49945439, 54162811, 54799116, 56096559, 58701489, 58831643, 59485171, 59722118, 59774524, 59885371, 59885737, 59886184, 59886506, 59888508, 59913655, 61039672 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 28373957,
                ProductIds = new long[] { 8007749, 8298595, 9605313, 10175243, 10225270, 10915323, 10990198, 12426383, 12984935, 15366691, 15531465, 17239627, 20375800, 20761600, 21621819, 23249555, 23650380, 24045326, 26323965, 26371178, 27064043, 31625853, 33681228, 40772605, 43230547, 44367901, 44399788, 44578737, 44779587, 49945439, 52472818, 52824658, 53402942, 53807601, 53944800, 57439880 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 201872887,
                ProductIds = new long[] { 191, 13124374, 13759128, 25678446, 28943527, 30327682, 34992854, 37186921, 38045694, 39671484, 41970188, 50011795, 51080186 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 291460130,
                ProductIds = new long[] { 9260177, 12153804, 13683203, 13830700, 15531485, 20330137, 26457182, 28943527, 37832564, 42514234, 43349537, 43851298, 43851409, 46110670, 47890140, 51952026, 52684244, 52709897, 53165326, 56892559, 57382442, 57744196, 58048847, 59304331, 59748429, 60634627, 60694335, 60770270, 62225266 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 122108564,
                ProductIds = new long[] { 4334399, 5728709, 5934878, 8674611, 12371627, 17113172, 17470724, 17471158, 17696282, 20383310, 20697477, 21115990, 22609588, 23332761, 33811688 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 11663082,
                ProductIds = new long[] { 80, 5962883, 10225401, 13683313, 18902141, 21177224, 21503254, 22943521, 23654522, 25331887, 33330924, 41514210, 41834327, 42797835, 46289531, 46692670, 48394393, 48766493, 49364837, 52372479, 52673089, 53465241, 55856350, 57530697, 59649612, 62099930, 62188105, 62222248, 62251168, 62251268 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 69310461,
                ProductIds = new long[] { 191, 1194764, 3239146, 4440815, 5002482, 6501320, 6724012, 6771601, 6824661, 7019215, 7611424, 11601686, 12742990, 13831517, 13905792, 17519619, 21337704, 22852510, 27759230, 33006135, 41532988, 42815277, 46152584, 46787801, 48567289, 48567909, 48568278, 56858836, 56966729, 57953883 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = -1,
                ProductIds = new long[] { 6070165, 6191906, 7090049, 72966682, 112963092, 14374890, 143891942, 18546838, 18941734, 231108715, 27901109, 28168483, 28168779, 34600329, 346004032, 346005963, 346006744, 34600802, 3460119916, 34601318, 346018312, 37364616, 42249134, 4501030710, 511350992, 538981422, 61796401 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 83079851,
                ProductIds = new long[] { 367079, 554785, 5442726, 5463434, 10029514, 10283184, 13831030, 22275204, 23704177, 29176619, 30734648, 30734662, 33025581, 33213036, 33753555, 38039409, 54048674, 54048687, 60398196 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 6250065,
                ProductIds = new long[] { 191, 14810201, 19737572, 28943527, 32007158, 38047006, 43362118, 45491967, 51977492, 53529598 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 350566200,
                ProductIds = new long[] { 19604157, 21415030, 22275353, 27273853, 28391663, 35874822, 45784973, 50325003, 62172952, 62185010, 62267109 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 147774626,
                ProductIds = new long[] { 3491836, 11206522, 13683313, 14550865, 15531453, 18347468, 23504253, 23858838, 24528986, 25140013, 28276809, 29548566, 29681346, 30100424, 31795297, 32745772, 33559176, 33846164, 34536385, 38976834, 40125169, 40135187, 41706048, 42412210, 42773039, 45122734, 46765502, 49566256, 50412449, 51263199, 51877622, 53807601, 53819971, 57486644, 57503249, 58076938, 59346134, 60757596 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 274878192,
                ProductIds = new long[] { 191, 4142040, 8676692, 13759128, 17458357, 26791992, 30309185, 31600774, 32324413, 36755576, 43159904, 43851250, 45278953, 58184682 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 240726623,
                ProductIds = new long[] { 80, 12929155, 13683313, 28785920, 30071761, 30108107, 32756579, 38614496, 39133718, 41629821, 42082170, 46127220, 46191755, 46452723, 50254067, 57956419 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 172966915,
                ProductIds = new long[] { 4558607, 6531070, 12077119, 18200551, 20375800, 21032520, 22275280, 23191442, 28252707, 29136968, 30060300, 30976027, 32584819, 33723486, 36152010, 39063135, 39868734, 40143438, 40556156, 40561852, 42368150, 42388839, 42505463, 44166642, 45047907, 45712633, 48002787, 48910274, 50522872, 57087610, 57151840, 58358403 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 3764101,
                ProductIds = new long[] { 4166396, 12327296, 12437819, 16070361, 20125329, 22275280, 40942574, 41633787, 43883874, 44595756, 45407757, 47984653, 49151266, 49433618, 53994647, 55280825, 56939388, 58103213, 61969309, 62218031 }
            });
            entries.Add(new ExposeOutfitsRequestEntry
            {
                AvatarId = 154060512,
	            ProductIds = new long[] { 2803406, 11664166, 13490523, 13683313, 15531441, 18994881, 19504797, 22854374, 30787177, 33604847, 33761269, 34630491, 36087822, 40125169, 41272964, 42050573, 44367901, 49945439, 53601667, 55244690, 55244726 }
            });
            return new ExposeOutfitsRequest { Entries = entries.ToArray() };
        }
    }
}
