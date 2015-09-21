using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mozu.Api;
using Autofac;
using Mozu.Api.ToolKit.Config;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Mozu_BED_Training_Exercise_11_1
{
    [TestClass]
    public class MozuDataConnectorTests
    {
        private IApiContext _apiContext;
        private IContainer _container;

        [TestInitialize]
        public void Init()
        {
            _container = new Bootstrapper().Bootstrap().Container;
            var appSetting = _container.Resolve<IAppSetting>();
            var tenantId = int.Parse(appSetting.Settings["TenantId"].ToString());
            var siteId = int.Parse(appSetting.Settings["SiteId"].ToString());

            _apiContext = new ApiContext(tenantId, siteId);
        }

        [TestMethod]
        public async Task Exercise_11_1_Get_Products()
        {
            //create a new product resource
            var productResource = new Mozu.Api.Resources.Commerce.Catalog.Admin.ProductResource(_apiContext);

            //Get products
            var products = (productResource.GetProductsAsync(startIndex: 0, pageSize: 200).Result);

            //Add Your Code: 
            //Write total number of products to output window
            System.Diagnostics.Debug.WriteLine("Total Products: {0}", products.TotalCount);

            //Add Your Code: 
            //Get all products that have options and are configurable
            var configurableProducts = products.Items.Where(d => d.Options != null).ToList();

            //Add Your Code: 
            //Write total number of configurable products to output window
            System.Diagnostics.Debug.WriteLine("Total Configurable Products: {0}", configurableProducts.Count);

            //Add Your Code: 
            //Get all products that do not have options and are not configurable
            var nonConfigurableProducts = products.Items.Where(d => d.Options == null).ToList();

            //Add Your Code: 
            //Write total number of non-configurable products to output window
            System.Diagnostics.Debug.WriteLine("Total Non-Configurable Products: {0}", nonConfigurableProducts.Count);

            //Add Your Code: 
            //Get all products that are scarfs
            var scarfProducts = products.Items.Where(d => d.Content.ProductName.ToLower().Contains("scarf")).ToList();

            //Add Your Code: 
            //Write total number of scarf products to output window
            System.Diagnostics.Debug.WriteLine("Total Scarf Products: {0}", scarfProducts.Count);

            //Add Your Code: 
            //Get product price
            var purseProduct = productResource.GetProductAsync("LUC-BAG-007").Result;

            //Add Your Code: 
            //Write product prices to output window
            System.Diagnostics.Debug.WriteLine("Product Prices[{0}]: Price({1}) Sales Price({2})", purseProduct.ProductCode, purseProduct.Price.Price, purseProduct.Price.SalePrice);

            //Create a new location inventory resource
            var inventoryResource = new Mozu.Api.Resources.Commerce.Catalog.Admin.LocationInventoryResource(_apiContext);

            //Add Your Code: 
            //Get inventory
            var inventory = inventoryResource.GetLocationInventoryAsync("WRH01", "LUC-BAG-007").Result;
            
            //Demostrate utility methods
            //var collectionsList =  await StoreMultipleProductCollections(productResource);

            //var products = ReturnProductsFromProductCollections(collectionsList);
                        
        }

        /// <summary>
        /// Helper method for returning multiple Product Collections if the page size is greater than 1
        /// </summary>
        /// <param name="productResource">Apicontext-driven </param>
        private async static Task<List<Mozu.Api.Contracts.ProductAdmin.ProductCollection>> StoreMultipleProductCollections(Mozu.Api.Resources.Commerce.Catalog.Admin.ProductResource productResource)
        {
            var productCollectionsList = new List<Mozu.Api.Contracts.ProductAdmin.ProductCollection>();
            var totalProductCount = 0;
            var startIndex = 0;
            var pageSize = 200;
            var productCollection = new Mozu.Api.Contracts.ProductAdmin.ProductCollection();

            do
            {
                productCollection = await productResource.GetProductsAsync(pageSize: pageSize, startIndex: startIndex);
                productCollectionsList.Add(productCollection);
                totalProductCount = productCollection.TotalCount;
                startIndex += pageSize;
            } while (totalProductCount > startIndex);

            return productCollectionsList;
        }

        /// <summary>
        /// Helper method breaking multiple ProductCollections into a List<Products>
        /// </summary>
        /// <param name="productCollectionList">A List<ProductCollection></param>
        private static List<Mozu.Api.Contracts.ProductAdmin.Product> ReturnProductsFromProductCollections(List<Mozu.Api.Contracts.ProductAdmin.ProductCollection> productCollectionList)
        {
            var allProducts = new List<Mozu.Api.Contracts.ProductAdmin.Product>();
            foreach(var list in productCollectionList)
            {
                foreach(var product in list.Items)
                {
                    allProducts.Add(product);
                }
            }

            return allProducts;
        }
    }
}
