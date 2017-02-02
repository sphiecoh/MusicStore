var express = require('express'),
  router = express.Router(),
  db = require('../models');

module.exports = function (app) {
  app.use('/shipping', router);
};

router.get('/', function (req, res, next) {
  db.Shipment.findAll().then(function (shipments) {
    res.json(shipments);
  });
});
router.post('/',function (req, res, next) {
    var shipment = {};      // create a new instance of the Bear model
    shipment.address = req.body.address;
    shipment.status = 'recieved';
    shipment.userid =  req.body.userid;
    shipment.ordernumber = req.body.ordernumber;
    db.Shipment.create(shipment).then(function (shipment) {
    res.json(shipment);
  });
});
router.get('/:id', function (req, res, next) {
  db.Shipment.findOne({
  where: {
    id: req.params.id
  }
}).then(function (shipment) {
    res.json(shipment);
  });
});
