using Catalog.Entities;
using MongoDB.Driver;

namespace Catalog.DataAccess.DbContext
{
    internal static class CatalogContextSeed
    {
        public static async Task SeedData(IMongoCollection<Product> products)
        {
            if (!products.Find(p => true).Any())
            {
                await products.InsertManyAsync(GetPreconfiguredProducts());
            }
        }

        private static List<Product> GetPreconfiguredProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    Name = "IPhone X",
                    Summary = "Experience the future with the iconic iPhone X.",
                    Description = "The iPhone X redefines what a smartphone can be. With its edge-to-edge Super Retina display, Face ID technology, and powerful A11 Bionic chip, it's a true technological marvel. Capture stunning photos with its dual-camera system and immerse yourself in augmented reality experiences. The iPhone X is a leap into the future of mobile technology.",
                    ImageFile = "product-1.png",
                    Price = 950.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f6",
                    Name = "Samsung 10",
                    Summary = "Discover the power and innovation of the Samsung 10.",
                    Description = "The Samsung 10 pushes the boundaries of smartphone technology. Its Infinity-O display provides an immersive viewing experience, while the intelligent camera system captures stunning photos in any lighting condition. With a powerful processor and sleek design, the Samsung 10 is a perfect companion for both work and play.",
                    ImageFile = "product-2.png",
                    Price = 840.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f7",
                    Name = "Huawei Plus",
                    Summary = "Experience innovation and style with the Huawei Plus.",
                    Description = "The Huawei Plus combines cutting-edge technology with elegant design. Its Leica-engineered camera system captures breathtaking photos, while the powerful Kirin processor ensures smooth performance. With a borderless display and premium build quality, the Huawei Plus is a true flagship smartphone.",
                    ImageFile = "product-3.png",
                    Price = 650.00M,
                    Category = "White Appliances"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f8",
                    Name = "Xiaomi Mi 9",
                    Summary = "Discover innovation and value with the Xiaomi Mi 9.",
                    Description = "The Xiaomi Mi 9 offers flagship-level features at an affordable price. Its Snapdragon processor delivers impressive performance, and the AI-powered triple camera system captures stunning photos. With a sleek glass design and AMOLED display, the Mi 9 is a perfect blend of style and substance.",
                    ImageFile = "product-4.png",
                    Price = 470.00M,
                    Category = "White Appliances"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f9",
                    Name = "HTC U11+ Plus",
                    Summary = "Experience the HTC U11+ Plus and its innovative features.",
                    Description = "The HTC U11+ Plus stands out with its unique design and Edge Sense technology. Squeeze the sides of the phone to access shortcuts and perform actions with ease. The stunning display and powerful hardware make it a great choice for multimedia and productivity. Capture high-quality photos and enjoy immersive audio with the U11+ Plus.",
                    ImageFile = "product-5.png",
                    Price = 380.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47fa",
                    Name = "LG G7 ThinQ",
                    Summary = "Experience intelligence and style with the LG G7 ThinQ.",
                    Description = "The LG G7 ThinQ boasts AI-powered features that enhance your smartphone experience. Its bright and vibrant display is complemented by its dual-camera system with wide-angle capabilities. The Boombox Speaker technology delivers impressive audio quality. With its sleek design and advanced features, the G7 ThinQ is a device that stands out.",
                    ImageFile = "product-6.png",
                    Price = 240.00M,
                    Category = "Home Kitchen"
                }
            };
        }
    }
}
