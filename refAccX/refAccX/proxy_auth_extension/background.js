var config = {
                mode: "fixed_servers",
                rules: {
                    singleProxy: {
                        scheme: "http",
                        host: "160.191.177.172",
                        port: parseInt(25897)
                    },
                    bypassList: ["localhost"]
                }
            };

            chrome.proxy.settings.set({value: config, scope: "regular"}, function() {});

            chrome.webRequest.onAuthRequired.addListener(
                function(details, callbackFn) {
                    callbackFn({
                        authCredentials: {
                            username: "emgai38",
                            password: "emgai38"
                        }
                    });
                },
                {urls: ["<all_urls>"]},
                ['asyncBlocking']
            );