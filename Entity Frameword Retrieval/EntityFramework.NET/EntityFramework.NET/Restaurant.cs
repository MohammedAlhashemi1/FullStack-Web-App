using ICA10.NET.Models;

namespace ICA10.NET
{
    public static class Restaurant
    {
        public static object GetLocations()
        {
            using var db = new Malhashemi1RestaurantDbContext();

            var locs = db.Locations
                .OrderBy(l => l.LocationName)
                .Select(l => new {
                    locationId = l.Locationid,
                    locationName = l.LocationName
                })
                .ToList();

            return locs;
        }

        public static object GetOrders(int customerId, int locationId)
        {
            using var db = new Malhashemi1RestaurantDbContext();

            var cust = db.Customers
                .Where(c => c.Cid == customerId)
                .Select(c => c.Fname + " " + c.Lname)
                .FirstOrDefault();

            if (cust == null)
                return new { error = "Customer not found." };

            var loc = db.Locations
                .Where(l => l.Locationid == locationId)
                .Select(l => l.LocationName)
                .FirstOrDefault();

            if (loc == null)
                return new { error = "Location not found." };

            var orders = db.Orders
                .Where(o => o.Cid == customerId && o.Locationid == locationId)
                .Select(o => new {
                    orderId = o.OrderId,
                    orderDate = o.OrderDate,
                    paymentMethod = o.PaymentMethod,
                    itemName = o.Item.ItemName,
                    itemPrice = o.Item.ItemPrice,
                    itemCount = o.ItemCount
                })
                .OrderBy(o => o.orderId)
                .ToList();

            return new
            {
                customerName = cust,
                locationName = loc,
                orders = orders
            };
        }
    }
}
