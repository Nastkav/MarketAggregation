using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MyShop.Helpers
{
    public static class Extension
    {
        private static Random rng = new Random();

        public static Dictionary<string, ComparsionModel> comparison = new Dictionary<string, ComparsionModel>
        {
            { "vegetables", new ComparsionModel() 
                {
                    Name = "Овочі",
                    AtbCategoryName = "289-ovochi",
                    VarusCategoryName = "frukti-ovochi-gorihi/ovochi-svizhi",
                    MetroCategoryName = "vegetables-metro/",
                    SilpoCategoryName = "ovochi-378"
                } 
            },
            { "fruit", new ComparsionModel() 
                {
                    Name = "Фрукти",
                    AtbCategoryName = "288-frukti-yagodi",
                    VarusCategoryName = "frukti-ovochi-gorihi/frukti-svizhi",
                    MetroCategoryName = "fruits-metro/",
                    SilpoCategoryName = "frukty-381"
                }
            },
            { "milk", new ComparsionModel()
                {
                    Name = "Молоко",
                    AtbCategoryName = "398-moloko",
                    VarusCategoryName = "molochni-produkti/moloko",
                    MetroCategoryName = "milk-metro/",
                    SilpoCategoryName = "moloko-253"
                }
            },
            { "sausage", new ComparsionModel()
                {
                    Name = "Ковбаса",
                    AtbCategoryName = "360-kovbasa-i-m-yasni-delikatesi",
                    VarusCategoryName = "kolbasy-sosiski-delikatesy/kovbasi",
                    MetroCategoryName = "sausages-and-burgers-metro/",
                    SilpoCategoryName = "m-iaso-kovbasni-vyroby-316"
                }
            },
            { "meat", new ComparsionModel()
                {
                    Name = "М'ясо",
                    AtbCategoryName = "343-m-yaso-ta-yaytsya",
                    VarusCategoryName = "myasni-virobi-ta-yaycya/myaso-svizhe",
                    MetroCategoryName = "fresh-meat-metro/",
                    SilpoCategoryName = "svizhe-m-iaso-278"
                }
            },
            { "fish", new ComparsionModel()
                {
                    Name = "Риба",
                    AtbCategoryName = "353-riba-i-moreprodukti",
                    VarusCategoryName = "riba-ta-moreprodukti/riba",
                    MetroCategoryName = "fresh-fish-metro/",
                    SilpoCategoryName = "zhyva-ta-okholodzhena-ryba-ta-moreprodukty-280"
                }
            },
            { "chemical", new ComparsionModel()
                {
                    Name = "Побутова хімія",
                    AtbCategoryName = "308-pobutova-khimiya-ta-neprodovol-chi-tovari",
                    VarusCategoryName = "tovari-dlya-domu-ta-sadu/pobutova-himiya",
                    MetroCategoryName = "for-cleaning-metro/",
                    SilpoCategoryName = "pobutova-khimiia-576"
                }
            },
        };
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void KillProcess()
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("chrome"))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}