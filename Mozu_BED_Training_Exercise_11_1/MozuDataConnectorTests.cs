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
            var allProducts = await GetAllProducts(productResource);
                        
        }

        /// <summary>
        /// Helper method for returning a List<Products> from multiple Product Collections if the page size is greater than 1
        /// </summary>
        /// <param name="productResource">Apicontext-driven </param>
        private async static Task<List<Mozu.Api.Contracts.ProductAdmin.Product>> GetAllProducts(Mozu.Api.Resources.Commerce.Catalog.Admin.ProductResource productResource)
        {
            var productCollectionsTaskList = new List<Task<Mozu.Api.Contracts.ProductAdmin.ProductCollection>>();
            var productCollectionsList = new List<Mozu.Api.Contracts.ProductAdmin.ProductCollection>();
            var productsList = new List<Mozu.Api.Contracts.ProductAdmin.Product>();
            var totalProductCount = 0;
            var startIndex = 0;
            var pageSize = 1;

            var productCollection = await productResource.GetProductsAsync(pageSize: pageSize, startIndex: startIndex);
            totalProductCount = productCollection.TotalCount;
            startIndex += pageSize;
            productsList.AddRange(productCollection.Items);

            while (totalProductCount > startIndex)
            {
                productCollectionsTaskList.Add(productResource.GetProductsAsync(pageSize: pageSize, startIndex: startIndex));
                startIndex += pageSize;
            }

            while (productCollectionsTaskList.Count > 0)
            {
                var finishedTask = await Task.WhenAny(productCollectionsTaskList);
                productCollectionsTaskList.Remove(finishedTask);

                productsList.AddRange((await finishedTask).Items);
            }

            return productsList;
        }
    }
}
