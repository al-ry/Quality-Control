const mbHelper = require('./mountbankHelper');
const settings = require('./settings');

function addRates() {
    const USDResponse = { rate : "73.6" }
    const EURResponse = { rate : "89.5" }
    const UAHResponse = {rate: '2.3'}
    const BYNResponse = {rate: '29.4'}

    const stubs = [
        {
            predicates: [ {
                equals: {
                    method: "GET",
                    "path": "/rates/usd"
                }
            }],
            responses: [
                {
                    is: {
                        statusCode: 200,
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(USDResponse)
                    }
                }
            ]
        },
        {
            predicates: [ {
                equals: {
                    method: "GET",
                    "path": "/rates/eur"
                }
            }],
            responses: [
                {
                    is: {
                        statusCode: 200,
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(EURResponse)
                    }
                }
            ]
        },
        {
            predicates: [ {
                equals: {
                    method: "GET",
                    "path": "/rates/uah"
                }
            }],
            responses: [
                {
                    is: {
                        statusCode: 200,
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(UAHResponse)
                    }
                }
            ]
        },
        {
            predicates: [ {
                equals: {
                    method: "GET",
                    "path": "/rates/byn"
                }
            }],
            responses: [
                {
                    is: {
                        statusCode: 200,
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify(BYNResponse)
                    }
                }
            ]
        }
    ];

    const imposter = {
        port: settings.rates_mock_api,
        protocol: 'http',
        stubs: stubs
    };

    return mbHelper.postImposter(imposter);
}

module.exports = { addRates};