# Mara.Plugins.Consent

This plugin provides a mechanism for flagging commands which require consent to collect, store, and use personal
information.

To use this plugin, flag commands which use personal information with `[RequiresConsent]`. A condition is registered
with Remora that requires consent to have been granted, and if it has not, the consent service then automatically
requests consent. 