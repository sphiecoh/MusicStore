module.exports = function (sequelize, DataTypes) {

  var Shipment = sequelize.define('Shipment', {
    id: { type: DataTypes.INTEGER, autoIncrement: true ,primaryKey: true} ,
    address: DataTypes.STRING,
    status:{ type: DataTypes.ENUM , values: ['recieved', 'packing', 'shipped'],defaultValue: 'recieved'},
    userid : DataTypes.STRING,
    ordernumber: DataTypes.STRING
  }, {
    classMethods: {
      associate: function (models) {
        // example on how to add relations
        // Article.hasMany(models.Comments);
      }
    }
  });

  return Shipment;
};

