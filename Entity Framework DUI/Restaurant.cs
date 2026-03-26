using ICA11.NET.Models;

namespace ICA11.NET
{
    public static class Restaurant
    {
        /* ============================
       GET LOCATIONS
    ============================ */
        public static List<object> GetLocations()
        {
            using (var db = new Malhashemi1RestaurantDbContext())
            {
                return db.Locations
                    .OrderBy(l => l.LocationName)
                    .Select(l => new {
                        locationId = l.Locationid,
                        locationName = l.LocationName
                    })
                    .ToList<object>();
            }
        }
        /* ============================
           GET ITEMS
        ============================ */
        public static List<object> GetItems()
        {
            using (var db = new Malhashemi1RestaurantDbContext())
            {
                return db.Items
                    .OrderBy(i => i.ItemName)
                    .Select(i => new {
                        itemId = i.Itemid,
                        itemName = i.ItemName
                    })
                    .ToList<object>();
            }
        }
        /* ============================
           GET ORDERS
        ============================ */
        public static object GetOrders(int customerId, int locationId)
        {
            using (var db = new Malhashemi1RestaurantDbContext())
            {
                var customer = db.Customers.FirstOrDefault(c => c.Cid == customerId);

                if (customer == null)
                    return new { error = "Invalid customer ID." };

                var location = db.Locations.FirstOrDefault(l => l.Locationid == locationId);

                if (location == null)
                    return new { error = "Invalid location." };

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
                    .ToList();

                return new
                {
                    customerName = customer.Fname + " " + customer.Lname,
                    locationName = location.LocationName,
                    orders = orders
                };
            }
        }
        /* ============================
           DELETE ORDER
        ============================ */
        public static object DeleteOrder(int id)
        {
            using (var db = new Malhashemi1RestaurantDbContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.OrderId == id);

                if (order == null)
                    return new { error = "Order not found." };

                db.Orders.Remove(order);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.ChangeTracker.Clear();
                    return new { message = e.Message };
                }

                return new { message = "Order deleted." };
            }
        }
        /* ============================
           ADD ORDER
        ============================ */
        public static object AddOrder(OrderData d)
        {
            //Data Validation
            if (d == null)
                return new { error = "Invalid data received." };
            if (d.CustomerId <= 0)
                return new { error = "Customer ID is required." };
            if (d.ItemId <= 0)
                return new { error = "Item must be selected." };
            if (d.ItemCount <= 0)
                return new { error = "Item count must be at least 1." };
            if (string.IsNullOrWhiteSpace(d.Payment))
                return new { error = "Payment method is required." };
            if (d.LocationId <= 0)
                return new { error = "Pickup location is required." };


            using (var db = new Malhashemi1RestaurantDbContext())
            {
                var cust = db.Customers.FirstOrDefault(c => c.Cid == d.CustomerId);

                if (cust == null)
                    return new { error = "Invalid customer ID." };

                var order = new Order
                {
                    Cid = d.CustomerId,
                    Locationid = d.LocationId,
                    Itemid = d.ItemId,
                    ItemCount = d.ItemCount,
                    PaymentMethod = d.Payment,
                    OrderDate = DateTime.Now
                };

                db.Orders.Add(order);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.ChangeTracker.Clear();
                    return new { message = e.Message };
                }

                int ready = new Random().Next(5, 31);

                return new
                {
                    orderId = order.OrderId,
                    readyMins = ready
                };
            }
        }
        /* ============================
           UPDATE ORDER
        ============================ */
        public static object UpdateOrder(OrderUpdate u)
        {
            if (u == null)
                return new { error = "Invalid data received." };
            if (u.OrderId <= 0)
                return new { error = "Order ID is missing." };
            if (u.ItemId <= 0)
                return new { error = "Item must be selected." };
            if (u.ItemCount <= 0)
                return new { error = "Item count must be at least 1." };
            if (string.IsNullOrWhiteSpace(u.Payment))
                return new { error = "Payment method is required." };


            using (var db = new Malhashemi1RestaurantDbContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.OrderId == u.OrderId);

                if (order == null)
                    return new { error = "Order not found." };

                order.Itemid = u.ItemId;
                order.ItemCount = u.ItemCount;
                order.PaymentMethod = u.Payment;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.ChangeTracker.Clear();
                    return new { message = e.Message };
                }

                return new { message = "Order updated." };
            }
        }
    }
}

