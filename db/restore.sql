--
-- NOTE:
--
-- File paths need to be edited. Search for $$PATH$$ and
-- replace it with the path to the directory containing
-- the extracted data files.
--
--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5.5
-- Dumped by pg_dump version 9.5.5

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

SET search_path = public, pg_catalog;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- Name: add_album(integer, integer, character varying, numeric, character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION add_album(gid integer, aid integer, t character varying, p numeric, aurl character varying,q integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$

begin
	INSERT INTO albums(genreid,artistid,title,price,albumarturl,quantity)
    VALUES(gid,aid,t,p,aurl,q);
    RETURN (SELECT MAX(albumid) FROM public.albums);
end;

$$;


ALTER FUNCTION public.add_album(gid integer, aid integer, t character varying, p numeric, aurl character varying, q integer) OWNER TO pguser;

--
-- Name: add_cart_item(character varying, integer, integer, timestamp without time zone); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION add_cart_item(cid character varying, aid integer, num integer, cdate timestamp without time zone) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	INSERT INTO public.carts(cartid, albumid, count, datecreated)
	VALUES(cid, aid, num, cdate);
end;

$$;


ALTER FUNCTION public.add_cart_item(cid character varying, aid integer, num integer, cdate timestamp without time zone) OWNER TO pguser;

--
-- Name: add_order(timestamp without time zone, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, numeric); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION add_order(odate timestamp without time zone, uname character varying, fname character varying, lname character varying, adr character varying, cn character varying, sn character varying, pcode character varying, cname character varying, ph character varying, ea character varying, t numeric) RETURNS integer
    LANGUAGE plpgsql
    AS $$

begin
	INSERT INTO public.orders(orderdate, username, firstname, lastname, address, city, state, postalcode, country, phone, email, total)
	VALUES (odate, uname, fname, lname, adr, cn, sn, pcode, cname, ph, ea, t); 
    RETURN (SELECT MAX(orderid) FROM public.orders);
end;

$$;


ALTER FUNCTION public.add_order(odate timestamp without time zone, uname character varying, fname character varying, lname character varying, adr character varying, cn character varying, sn character varying, pcode character varying, cname character varying, ph character varying, ea character varying, t numeric) OWNER TO pguser;

--
-- Name: add_order_details(integer, integer, integer, numeric); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION add_order_details(oid integer, aid integer, qty integer, uprice numeric) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	INSERT INTO public.orderdetails(orderid, albumid, quantity, unitprice)
	VALUES (oid,aid, qty, uprice);
    update albums 
	set quantity = quantity - qty
	where albumid = aid;
end;

$$;


ALTER FUNCTION public.add_order_details(oid integer, aid integer, qty integer, uprice numeric) OWNER TO pguser;

--
-- Name: add_user(character varying, character varying, character varying, character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION add_user(uid character varying, uname character varying, upwd character varying, uemail character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	INSERT INTO public.sysuser(sysuserid, sysusername, sysuserpassword, sysuseremail)
	VALUES(uid, uname, md5(upwd), uemail);
end;

$$;


ALTER FUNCTION public.add_user(uid character varying, uname character varying, upwd character varying, uemail character varying) OWNER TO pguser;

--
-- Name: delete_album_by_aid(integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION delete_album_by_aid(aid integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$

begin
	DELETE FROM public.albums WHERE albumid=aid;
    RETURN (SELECT COUNT(*) FROM public.albums where albumid=aid);
end;

$$;


ALTER FUNCTION public.delete_album_by_aid(aid integer) OWNER TO pguser;

--
-- Name: delete_cart_item_by_cid(character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION delete_cart_item_by_cid(cid character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	DELETE FROM public.carts WHERE cartid =cid;
end;

$$;


ALTER FUNCTION public.delete_cart_item_by_cid(cid character varying) OWNER TO pguser;

--
-- Name: delete_cart_item_by_recordid(integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION delete_cart_item_by_recordid(rid integer) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	DELETE FROM public.carts WHERE recordid =rid;
end;

$$;


ALTER FUNCTION public.delete_cart_item_by_recordid(rid integer) OWNER TO pguser;

--
-- Name: get_album_by_aid(integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_album_by_aid(aid integer) RETURNS TABLE(title character varying, albumarturl character varying, albumid integer, price numeric, quantity integer)
    LANGUAGE plpgsql
    AS $$

begin
	RETURN QUERY SELECT 
    A.title,
    A.albumarturl,
    A.albumid,
    A.price  ,
    A.quantity  
    FROM albums A
    where A.albumid=aid;
end;

$$;


ALTER FUNCTION public.get_album_by_aid(aid integer) OWNER TO pguser;

--
-- Name: get_album_details_by_aid(integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_album_details_by_aid(aid integer) RETURNS TABLE(albumid integer, genreid integer, artistid integer, title character varying, price numeric, albumarturl character varying, genrename character varying, artistname character varying,quantity integer)
    LANGUAGE plpgsql
    AS $$
begin
	RETURN QUERY SELECT 
    A.albumid,
    A.genreid,
    A.artistid,
    A.title,
    A.price,
    A.albumarturl,
    G.name AS genrename,
    B.name AS artistname,
    A.quantity
    FROM albums A 
    INNER JOIN genres G ON A.genreid = G.genreid
    INNER JOIN artists B ON A.artistid = B.artistid
    WHERE A.albumid=aid;
end;
$$;


ALTER FUNCTION public.get_album_details_by_aid(aid integer) OWNER TO pguser;

--
-- Name: get_album_list_by_gname(character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_album_list_by_gname(gname character varying) RETURNS TABLE(title character varying, albumarturl character varying, albumid integer, genrename character varying)
    LANGUAGE plpgsql
    AS $$
begin
	RETURN QUERY SELECT 
    A.title,
    A.albumarturl,
    A.albumid,
    G.name AS genrename 
    FROM albums A
    INNER JOIN genres G ON A.genreid = G.genreid
    where G.name=gname;
end;
$$;


ALTER FUNCTION public.get_album_list_by_gname(gname character varying) OWNER TO pguser;

--
-- Name: get_album_title_by_recordid(integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_album_title_by_recordid(rid integer) RETURNS TABLE(title character varying)
    LANGUAGE plpgsql
    AS $$
begin
	RETURN QUERY SELECT 
    A.title 
    FROM public.carts C
	INNER JOIN albums A ON A.albumid = C.albumid
	where C.recordid=rid;
end;
$$;


ALTER FUNCTION public.get_album_title_by_recordid(rid integer) OWNER TO pguser;

--
-- Name: get_all_albums(); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_all_albums() RETURNS TABLE(albumid integer, title character varying, price numeric, genrename character varying, artistname character varying,quantity integer)
    LANGUAGE plpgsql
    AS $$

begin
	RETURN QUERY SELECT 
    A.albumid , 
    A.title , 
    A.price , 
    g.name AS genrename , 
    AR.name AS artistname,
    A.quantity
    FROM albums A
    INNER JOIN genres G ON A.genreid = G.genreid
    INNER JOIN artists AR ON AR.artistid = A.artistid;
end;

$$;


ALTER FUNCTION public.get_all_albums() OWNER TO pguser;

--
-- Name: get_all_artists(); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_all_artists() RETURNS TABLE(artistid integer, name character varying)
    LANGUAGE plpgsql
    AS $$

begin
	RETURN QUERY SELECT 
    A.artistid ,
    A.name 
    FROM artists A;
end;

$$;


ALTER FUNCTION public.get_all_artists() OWNER TO pguser;

--
-- Name: get_all_genres(); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_all_genres() RETURNS TABLE(genreid integer, name character varying, description character varying)
    LANGUAGE plpgsql
    AS $$
begin
	RETURN QUERY SELECT g.genreid, g.name, g.description FROM public.genres g;
end;
$$;


ALTER FUNCTION public.get_all_genres() OWNER TO pguser;

--
-- Name: get_cart_item_by_cartid_and_albumid(character varying, integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_cart_item_by_cartid_and_albumid(cid character varying, aid integer) RETURNS TABLE(recordid integer, cartid character varying, albumid integer, count integer, detecreated timestamp without time zone)
    LANGUAGE plpgsql
    AS $$
begin
	RETURN QUERY SELECT 
    C.recordid,
    C.cartid,
    C.albumid,
    C.count,
    C.datecreated
    FROM public.carts C 
    WHERE C.cartid = cid and C.albumid =aid 
    LIMIT 1;
end;

$$;


ALTER FUNCTION public.get_cart_item_by_cartid_and_albumid(cid character varying, aid integer) OWNER TO pguser;

--
-- Name: get_cart_item_by_cartid_and_recordid(character varying, integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_cart_item_by_cartid_and_recordid(cid character varying, rid integer) RETURNS TABLE(recordid integer, cartid character varying, albumid integer, count integer, detecreated timestamp without time zone)
    LANGUAGE plpgsql
    AS $$

begin
	RETURN QUERY SELECT 
    C.recordid,
    C.cartid,
    C.albumid,
    C.count,
    C.datecreated
    FROM public.carts C 
    WHERE C.cartid=cid and C.recordid=rid ;
end;

$$;


ALTER FUNCTION public.get_cart_item_by_cartid_and_recordid(cid character varying, rid integer) OWNER TO pguser;

--
-- Name: get_cart_item_by_cid(); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_cart_item_by_cid() RETURNS TABLE(recordid integer, cartid character varying, albumid integer, count integer, detecreated timestamp without time zone, title character varying, price numeric)
    LANGUAGE plpgsql
    AS $$
begin
	RETURN QUERY SELECT 
    C.recordid,
    C.cartid,
    C.albumid,
    C.count,
    C.datecreated,
    A.title,
    A.price 
    FROM carts C
	INNER JOIN albums A ON c.albumid = A.albumid
	WHERE C.cartid=cid; 
end;

$$;


ALTER FUNCTION public.get_cart_item_by_cid() OWNER TO pguser;

--
-- Name: get_cart_item_by_cid(character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_cart_item_by_cid(cid character varying) RETURNS TABLE(recordid integer, cartid character varying, albumid integer, count integer, detecreated timestamp without time zone, title character varying, price numeric)
    LANGUAGE plpgsql
    AS $$

begin
	RETURN QUERY SELECT 
    C.recordid,
    C.cartid,
    C.albumid,
    C.count,
    C.datecreated,
    A.title,
    A.price 
    FROM carts C
	INNER JOIN albums A ON c.albumid = A.albumid
	WHERE C.cartid=cid; 
end;

$$;


ALTER FUNCTION public.get_cart_item_by_cid(cid character varying) OWNER TO pguser;

--
-- Name: get_order_count_by_uname_and_orderid(integer, character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_order_count_by_uname_and_orderid(oid integer, uname character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$

begin
	RETURN (SELECT count(*) FROM public.orders O WHERE LOWER(O.username)=uname AND O.orderid=oid); 
end;

$$;


ALTER FUNCTION public.get_order_count_by_uname_and_orderid(oid integer, uname character varying) OWNER TO pguser;

--
-- Name: get_top_selling_albums(integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_top_selling_albums(num integer) RETURNS TABLE(albumid integer, genreid integer, artistid integer, title character varying, price numeric, albumarturl character varying)
    LANGUAGE plpgsql
    AS $$
begin        
    RETURN QUERY SELECT 
    A.albumid,
    A.genreid,
    A.artistid,
    A.title,
    A.price,
    A.albumarturl 
    FROM albums A
    LEFT JOIN orderdetails O ON A.albumid = O.albumid
    GROUP BY A.albumid,A.genreid,A.artistid,A.title,A.price,A.albumarturl
    ORDER BY count(O.albumid) desc LIMIT num;          
end;
$$;


ALTER FUNCTION public.get_top_selling_albums(num integer) OWNER TO pguser;

--
-- Name: get_total_count_by_cartid(character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_total_count_by_cartid(cid character varying) RETURNS numeric
    LANGUAGE plpgsql
    AS $$

begin
	RETURN (SELECT 
    CASE 
    	WHEN SUM(C.count) IS NULL THEN 0 
        ELSE SUM(C.count) 
    END AS res 
    FROM carts C
    WHERE C.cartid  = cid);
end;

$$;


ALTER FUNCTION public.get_total_count_by_cartid(cid character varying) OWNER TO pguser;

--
-- Name: get_total_order_by_cartid(character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_total_order_by_cartid(cid character varying) RETURNS numeric
    LANGUAGE plpgsql
    AS $$

begin
	RETURN (SELECT 
    SUM(C.count * A.price) 
    FROM public.carts C 
    INNER JOIN public.albums A ON C.albumid = A.albumid
    WHERE C.cartid = cid);
end;

$$;


ALTER FUNCTION public.get_total_order_by_cartid(cid character varying) OWNER TO pguser;

--
-- Name: get_user_by_name_and_password(character varying, character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_user_by_name_and_password(uname character varying, upwd character varying) RETURNS TABLE(sysuserid character varying, sysusername character varying, sysuserpassword character varying, sysuseremail character varying)
    LANGUAGE plpgsql
    AS $$
begin
	RETURN QUERY SELECT 
    U.sysuserid,
    U.sysusername,
    U.sysuserpassword,
    U.sysuseremail
    FROM public.sysuser U 
    WHERE U.sysusername = uname AND U.sysuserpassword = md5(upwd);
end;
$$;


ALTER FUNCTION public.get_user_by_name_and_password(uname character varying, upwd character varying) OWNER TO pguser;

--
-- Name: get_user_by_userid(character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION get_user_by_userid(uid character varying) RETURNS TABLE(sysuserid character varying, sysusername character varying, sysuserpassword character varying, sysuseremail character varying)
    LANGUAGE plpgsql
    AS $$

begin
	RETURN QUERY SELECT  
    S.sysuserid,
    S.sysusername,
    S.sysuserpassword,
    S.sysuseremail
    FROM public.sysuser S
    WHERE S.sysuserid=uid;
end;

$$;


ALTER FUNCTION public.get_user_by_userid(uid character varying) OWNER TO pguser;

--
-- Name: update_album_by_aid(integer, integer, integer, character varying, numeric, character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION update_album_by_aid(aid integer, gid integer, arid integer, t character varying, p numeric, aurl character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	UPDATE public.albums
	SET genreid=gid, artistid=arid, title=t, price=p, albumarturl=aurl
	WHERE albumid=aid;    
end;

$$;


ALTER FUNCTION public.update_album_by_aid(aid integer, gid integer, arid integer, t character varying, p numeric, aurl character varying) OWNER TO pguser;

--
-- Name: update_cart_count_by_recordid(integer, integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION update_cart_count_by_recordid(rid integer, num integer) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	UPDATE public.carts	
    SET count=num
	WHERE recordid=rid;
end;

$$;


ALTER FUNCTION public.update_cart_count_by_recordid(rid integer, num integer) OWNER TO pguser;

--
-- Name: update_cart_item(character varying, integer, integer); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION update_cart_item(cid character varying, aid integer, num integer) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	UPDATE public.carts 
    SET count=num
    WHERE  cartid = cid and albumid =aid;
end;

$$;


ALTER FUNCTION public.update_cart_item(cid character varying, aid integer, num integer) OWNER TO pguser;

--
-- Name: update_cartid_by_recordids(character varying, character varying); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION update_cartid_by_recordids(ocid character varying, ncid character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	UPDATE public.carts
	SET cartid=ncid
	WHERE recordid IN 
    (
    	SELECT recordid FROM public.carts C WHERE C.cartid=ocid
    );
end;

$$;


ALTER FUNCTION public.update_cartid_by_recordids(ocid character varying, ncid character varying) OWNER TO pguser;

--
-- Name: update_order_total_by_orderid(integer, numeric); Type: FUNCTION; Schema: public; Owner: pguser
--

CREATE FUNCTION update_order_total_by_orderid(oid integer, t numeric) RETURNS void
    LANGUAGE plpgsql
    AS $$

begin
	UPDATE public.orders SET  total=t WHERE orderid=oid;
end;

$$;


ALTER FUNCTION public.update_order_total_by_orderid(oid integer, t numeric) OWNER TO pguser;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: albums; Type: TABLE; Schema: public; Owner: pguser
--

CREATE TABLE albums (
    albumid integer NOT NULL,
    genreid integer NOT NULL,
    artistid integer NOT NULL,
    title character varying(160) NOT NULL,
    price numeric(18,2) NOT NULL,
    albumarturl character varying(1024),
    quantity INTEGER
);


ALTER TABLE albums OWNER TO pguser;

--
-- Name: albums_albumid_seq; Type: SEQUENCE; Schema: public; Owner: pguser
--

CREATE SEQUENCE albums_albumid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE albums_albumid_seq OWNER TO pguser;

--
-- Name: albums_albumid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: pguser
--

ALTER SEQUENCE albums_albumid_seq OWNED BY albums.albumid;


--
-- Name: artists; Type: TABLE; Schema: public; Owner: pguser
--

CREATE TABLE artists (
    artistid integer NOT NULL,
    name character varying(4000)
);


ALTER TABLE artists OWNER TO pguser;

--
-- Name: artists_artistid_seq; Type: SEQUENCE; Schema: public; Owner: pguser
--

CREATE SEQUENCE artists_artistid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE artists_artistid_seq OWNER TO pguser;

--
-- Name: artists_artistid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: pguser
--

ALTER SEQUENCE artists_artistid_seq OWNED BY artists.artistid;


--
-- Name: carts; Type: TABLE; Schema: public; Owner: pguser
--

CREATE TABLE carts (
    recordid integer NOT NULL,
    cartid character varying(4000),
    albumid integer NOT NULL,
    count integer NOT NULL,
    datecreated timestamp without time zone NOT NULL
);


ALTER TABLE carts OWNER TO pguser;

--
-- Name: carts_recordid_seq; Type: SEQUENCE; Schema: public; Owner: pguser
--

CREATE SEQUENCE carts_recordid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE carts_recordid_seq OWNER TO pguser;

--
-- Name: carts_recordid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: pguser
--

ALTER SEQUENCE carts_recordid_seq OWNED BY carts.recordid;


--
-- Name: edmmetadata; Type: TABLE; Schema: public; Owner: pguser
--

CREATE TABLE edmmetadata (
    id integer NOT NULL,
    modelhash character varying(4000)
);


ALTER TABLE edmmetadata OWNER TO pguser;

--
-- Name: edmmetadata_id_seq; Type: SEQUENCE; Schema: public; Owner: pguser
--

CREATE SEQUENCE edmmetadata_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE edmmetadata_id_seq OWNER TO pguser;

--
-- Name: edmmetadata_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: pguser
--

ALTER SEQUENCE edmmetadata_id_seq OWNED BY edmmetadata.id;


--
-- Name: genres; Type: TABLE; Schema: public; Owner: pguser
--

CREATE TABLE genres (
    genreid integer NOT NULL,
    name character varying(4000),
    description character varying(4000)
);


ALTER TABLE genres OWNER TO pguser;

--
-- Name: genres_genreid_seq; Type: SEQUENCE; Schema: public; Owner: pguser
--

CREATE SEQUENCE genres_genreid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE genres_genreid_seq OWNER TO pguser;

--
-- Name: genres_genreid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: pguser
--

ALTER SEQUENCE genres_genreid_seq OWNED BY genres.genreid;


--
-- Name: orderdetails; Type: TABLE; Schema: public; Owner: pguser
--

CREATE TABLE orderdetails (
    orderdetailid integer NOT NULL,
    orderid integer NOT NULL,
    albumid integer NOT NULL,
    quantity integer NOT NULL,
    unitprice numeric(18,2) NOT NULL
);


ALTER TABLE orderdetails OWNER TO pguser;

--
-- Name: orderdetails_orderdetailid_seq; Type: SEQUENCE; Schema: public; Owner: pguser
--

CREATE SEQUENCE orderdetails_orderdetailid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE orderdetails_orderdetailid_seq OWNER TO pguser;

--
-- Name: orderdetails_orderdetailid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: pguser
--

ALTER SEQUENCE orderdetails_orderdetailid_seq OWNED BY orderdetails.orderdetailid;


--
-- Name: orders; Type: TABLE; Schema: public; Owner: pguser
--

CREATE TABLE orders (
    orderid integer NOT NULL,
    orderdate timestamp without time zone NOT NULL,
    username character varying(4000),
    firstname character varying(160) NOT NULL,
    lastname character varying(160) NOT NULL,
    address character varying(70) NOT NULL,
    city character varying(40) NOT NULL,
    state character varying(40) NOT NULL,
    postalcode character varying(10) NOT NULL,
    country character varying(40) NOT NULL,
    phone character varying(24) NOT NULL,
    email character varying(4000) NOT NULL,
    total numeric(18,2) NOT NULL,
    shippingid integer
);


ALTER TABLE orders OWNER TO pguser;

--
-- Name: orders_orderid_seq; Type: SEQUENCE; Schema: public; Owner: pguser
--

CREATE SEQUENCE orders_orderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE orders_orderid_seq OWNER TO pguser;

--
-- Name: orders_orderid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: pguser
--

ALTER SEQUENCE orders_orderid_seq OWNED BY orders.orderid;


--
-- Name: sysuser; Type: TABLE; Schema: public; Owner: pguser
--

CREATE TABLE sysuser (
    sysuserid character varying(100) NOT NULL,
    sysusername character varying(100) NOT NULL,
    sysuserpassword character varying(100) NOT NULL,
    sysuseremail character varying(100)
);


ALTER TABLE sysuser OWNER TO pguser;

--
-- Name: albumid; Type: DEFAULT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY albums ALTER COLUMN albumid SET DEFAULT nextval('albums_albumid_seq'::regclass);


--
-- Name: artistid; Type: DEFAULT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY artists ALTER COLUMN artistid SET DEFAULT nextval('artists_artistid_seq'::regclass);


--
-- Name: recordid; Type: DEFAULT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY carts ALTER COLUMN recordid SET DEFAULT nextval('carts_recordid_seq'::regclass);


--
-- Name: id; Type: DEFAULT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY edmmetadata ALTER COLUMN id SET DEFAULT nextval('edmmetadata_id_seq'::regclass);


--
-- Name: genreid; Type: DEFAULT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY genres ALTER COLUMN genreid SET DEFAULT nextval('genres_genreid_seq'::regclass);


--
-- Name: orderdetailid; Type: DEFAULT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY orderdetails ALTER COLUMN orderdetailid SET DEFAULT nextval('orderdetails_orderdetailid_seq'::regclass);


--
-- Name: orderid; Type: DEFAULT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY orders ALTER COLUMN orderid SET DEFAULT nextval('orders_orderid_seq'::regclass);

--
-- Name: albums_pkey; Type: CONSTRAINT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY albums
    ADD CONSTRAINT albums_pkey PRIMARY KEY (albumid);


--
-- Name: artists_pkey; Type: CONSTRAINT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY artists
    ADD CONSTRAINT artists_pkey PRIMARY KEY (artistid);


--
-- Name: carts_pkey; Type: CONSTRAINT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY carts
    ADD CONSTRAINT carts_pkey PRIMARY KEY (recordid);


--
-- Name: edmmetadata_pkey; Type: CONSTRAINT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY edmmetadata
    ADD CONSTRAINT edmmetadata_pkey PRIMARY KEY (id);


--
-- Name: genres_pkey; Type: CONSTRAINT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY genres
    ADD CONSTRAINT genres_pkey PRIMARY KEY (genreid);


--
-- Name: orderdetails_pkey; Type: CONSTRAINT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY orderdetails
    ADD CONSTRAINT orderdetails_pkey PRIMARY KEY (orderdetailid);


--
-- Name: orders_pkey; Type: CONSTRAINT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (orderid);


--
-- Name: sysuser_pkey; Type: CONSTRAINT; Schema: public; Owner: pguser
--

ALTER TABLE ONLY sysuser
    ADD CONSTRAINT sysuser_pkey PRIMARY KEY (sysuserid);


--
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

