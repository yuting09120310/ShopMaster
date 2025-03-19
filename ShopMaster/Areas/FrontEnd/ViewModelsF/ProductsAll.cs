﻿using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;


namespace ShopMaster.Areas.FrontEnd.ViewModelsF
{
    public class ProductsAll
    {
        //分類商品
        public List<IGrouping<int, Products>> ProductList { get; set; } = new List<IGrouping<int, Products>>(); 

        //喜愛商品
        public List<Products> ProductListLove { get; set; } = new List<Products>();

        //商品列表
        public  List<Products> ProductDetails { get; set; } = new List<Products>();

        //商品規格下拉選單
        public List<ProductType> ProductTypeList { get; set; } = new List<ProductType>();
    }}
