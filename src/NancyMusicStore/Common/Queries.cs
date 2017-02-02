namespace NancyMusicStore.Common
{
    public static class Queries
    {
        public const string AddOrderShippingId = @"update public.orders
                                                   set shippingid = @shipno
                                                   where orderid = @oid;";
        public const string GetLastOrderAddressByUsername = @"select * from orders
                                                              where username = @username
                                                              order by orderdate desc
                                                              limit 1;";
    }
}