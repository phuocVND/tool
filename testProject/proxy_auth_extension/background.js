var config = {
                mode: "fixed_servers",
                rules: {
                    singleProxy: {
                        scheme: "http",
                        host: "157.66.252.83",
                        port: parseInt(40471)
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