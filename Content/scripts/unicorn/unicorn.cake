#addin "Cake.Powershell"

Func<string, string> getUnicornSecret = (unicornConfigPath) => {
    if (FileExists(unicornConfigPath))
    {
        return XmlPeek(
            unicornConfigPath,
            "//configuration/sitecore/unicorn/authenticationProvider/SharedSecret"
        );
    }

    return null;
};

Func<string, string> getSiteUrlFromPublishSettings = (srcRoot) => {
    var file = $"{srcRoot}/publishsettings.targets";

    if (FileExists(file))
    {
        return XmlPeek(
            $"{srcRoot}/publishsettings.targets",
            "//*[local-name() = 'publishUrl']"
        );
    }

    return null;
};

Action<string, string, string, string> runUnicornSync = (siteUrl, secret, scriptDir, configurations) =>
{
    var url = $"{siteUrl.Trim('/')}/unicorn.aspx";

    StartPowershellFile(
        $"{scriptDir}/Sync.ps1",
        new PowershellSettings()
            .WithArguments(args => {
                args.Append("url", url).Append("secret", secret);

                if (!string.IsNullOrEmpty(scriptDir))
                {
                    args.Append("scriptDir", scriptDir);
                }
                if (!string.IsNullOrEmpty(configurations))
                {
                    args.Append("Configurations", configurations);
                }
            })
    );
};
