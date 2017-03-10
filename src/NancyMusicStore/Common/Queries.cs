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
        public const string GetOrderList = @"select orderid,orderdate,total from orders
                                            where username = @username
                                            order by orderdate desc";

        public const string GetOrderDetails = @"
                  select * from orders
                  where orderid = @oid;

                select o.quantity,o.unitprice as price,title,ar.name as artist,g.name as genre from
                orderdetails o
                join albums a on o.albumid = a.albumid
                join artists ar on ar.artistid = a.artistid
                join genres g on g.genreid = a.genreid
                where orderid = @oid;          
        ";
    }
}