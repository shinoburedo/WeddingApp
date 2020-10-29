using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MauloaDemo.Customer.Controllers;
using MauloaDemo.Repository;
using MauloaDemo.Models;
using MauloaDemo.Customer.ViewModels;

namespace MauloaDemo.Customer {

    public static class MasterDataHelpers {

        //public static List<SelectListItem> GetAreaList(string region_cd, bool includeBlank = true) {
        //    var repository = new AreaRepository();
        //    var areas = repository.GetList()
        //        .OrderBy(m => m.area_seq)
        //        .ThenBy(m => m.area_cd)
        //        .Take(1000);

        //    var list = new List<SelectListItem>();
        //    if (includeBlank) {
        //        list.Add(new SelectListItem { Text = "", Value = "", Selected = true });
        //    }

        //    foreach (var area in areas) {
        //        var item = new SelectListItem();
        //        item.Value = area.area_cd;
        //        item.Text = area.desc_eng;
        //        list.Add(item);
        //    }
        //    return list;
        //}

        //public static List<SelectListItem> GetAgentList(string region_cd, bool includeBlank = true) {
        //    var repository = new AgentRepository(region_cd);
        //    var agents = repository.GetList()
        //        .OrderBy(m => m.agent_cd)
        //        .Where(i => i.stop_tran_date == null)   // TODO:ディスコンの切り替え方を考える(CHECKBOX?)
        //        .Take(1000);

        //    var agentList = new List<SelectListItem>();
        //    if (includeBlank) {
        //        agentList.Add(new SelectListItem { Text = "", Value = "", Selected = true });
        //    }

        //    foreach (var agent in agents) {
        //        var item = new SelectListItem();
        //        item.Value = agent.area_cd;
        //        item.Text = agent.agent_name;
        //        agentList.Add(item);
        //    }
        //    return agentList;
        //}

        //public static List<SelectListItem> GetHotelList(string region_cd, bool includeBlank = true) {
        //    var repository = new HotelRepository(region_cd);
        //    var hotels = repository.GetList()
        //        .OrderBy(m => m.hotel_cd)
        //        .Take(1000);

        //    var hotelList = new List<SelectListItem>();
        //    if (includeBlank) {
        //        hotelList.Add(new SelectListItem { Text = "", Value = "", Selected = true });
        //    }

        //    foreach (var hotel in hotels) {
        //        var item = new SelectListItem();
        //        item.Value = hotel.hotel_cd;
        //        item.Text = hotel.hotel_name;
        //        hotelList.Add(item);
        //    }
        //    return hotelList;
        //}

    }
}