using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
        public class DriverViewModel : IDisposable
        {
            public DriverViewModel(Driver model)
            {
                Id = model.Id;
                FullName = model.FullName;
                Email = model.Email;
                Phone = model.Phone;
                City = model.City;
                VehicleType = model.VehicleType;
                HearFrom = model.HearFrom;
               
            }
     
        public int Id { get; set; }    

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string VehicleType { get; set; }

        public string HearFrom { get; set; }

        public object Token { get; internal set; }

        public void Dispose()
            {
            }
        }
    }
