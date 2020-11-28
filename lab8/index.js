const mb = require('mountebank');
const settings = require('./settings');
const rates = require('./rates')

const mbServerInstance = mb.create({
        port: settings.port,
        pidfile: '../mb.pid',
        logfile: '../mb.log',
        protofile: '../protofile.json',
        ipWhitelist: ['*']
    });

mbServerInstance.then(function() {
    rates.addRates()
});