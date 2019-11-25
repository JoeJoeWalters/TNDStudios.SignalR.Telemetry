var tndStudios = tndStudios || {};
tndStudios.models = tndStudios.models || {};
tndStudios.models.telemetry =
    {
        // Telemetry Page Model
        page: function () {

            // The properties of the object            
            this.applications = new tndStudios.models.telemetry.applications(null); // The applications object
        },

        // Applications Model
        applications: function (data) {

            // The list of applications
            this.applicationArray = [];

            // Copy the content of this list of applications from search json package
            this.fromObject = function (fromObject) {

                // Clear the object first (just in case)
                this.clear();

                // Start copying the data from the other object
                this.applicationArray = fromObject;
            }

            // Clear this applications object (i.e. make it ready for use)
            this.clear = function () {

                // Clear the properties
                this.applicationArray = [];
            }

            // Any data passed in?
            if (data) {
                this.fromObject(data); // Assign the data to this object
            }
        },
    };