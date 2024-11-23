<template>
    <a-card title="Database Connection" class="database-connection" :bordered="false">
        <!-- Connection Info Section -->
        <template v-if="connectionInfo">
            <a-row>
                <a-col :span="24">
                    <p>
                        <strong>Database Type:</strong> {{ connectionInfo.DbType }}
                    </p>
                    <p>
                        <strong>Database:</strong> {{ connectionInfo.Database }}
                    </p>
                    <p>
                        <strong>Host:</strong> {{ connectionInfo.Host }}
                    </p>
                    <p>
                        <strong>Port:</strong> {{ connectionInfo.Port }}
                    </p>
                    <p>
                        <strong>User:</strong> {{ connectionInfo.User }}
                    </p>
                </a-col>
                <a-col :span="24" style="margin-top: 20px;">
                    <a-button type="primary" @click="testConnectionFromConfigs">Test Connection</a-button>
                </a-col>
            </a-row>
        </template>

        <!-- Connection Form Section -->
        <template v-else>
            <a-form layout="vertical" @submit.prevent="onSubmit">
                <a-form-item label="Database Type">
                    <a-select v-model="selectedDbType" :options="dbTypes" placeholder="Select Database Type" />
                </a-form-item>
                <a-form-item label="Connection String">
                    <a-input v-model="connectionString" :placeholder="getPlaceholderForDbType(selectedDbType)" />
                </a-form-item>
                <a-form-item>
                    <a-button type="primary" @click="testConnection">Test Connection</a-button>
                    <a-button style="margin-left: 10px;" @click="saveConnectionInfo">Save</a-button>
                </a-form-item>
            </a-form>
        </template>
    </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import {
    fetchConnectionInfo,
    fetchDbTypes,
    tryConnect,
    saveConnection,
} from "@/services/connectionService";
import type { ConnectionInfoViewModel } from "@/services/connectionService";

// Reactive data
const connectionInfo = ref<ConnectionInfoViewModel | null>(null);
const dbTypes = ref<{ value: number; label: string }[]>([]);
const selectedDbType = ref<number | null>(null);
const connectionString = ref<string>("");

// Fetch placeholder for selected DB type
const getPlaceholderForDbType = (dbType: number | null) => {
    switch (dbType) {
        case 0: // SQL_ANYWHERE
            return "e.g., Anywhere:ENG=server_name;DBN=database_name;UID=user;PWD=password;";
        case 1: // SQL_SERVER
            return "e.g., Server=server_name;Database=database_name;User Id=user;Password=password;";
        case 2: // ORACLE
            return "e.g., (DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=host)(PORT=port))(CONNECT_DATA=(SERVICE_NAME=service_name)))";
        case 3: // MYSQL
            return "e.g., mysql://user:password@host:port/database";
        case 4: // POSTGRES
            return "e.g., postgresql://user:password@host:port/database";
        default:
            return "Enter connection string...";
    }
};

// Load connection info and database types
const loadConnectionSettings = async () => {
    try {
        connectionInfo.value = await fetchConnectionInfo();
        if (!connectionInfo.value) {
            dbTypes.value = await fetchDbTypes();
        }
    } catch (error) {
        console.error("Error loading connection settings:", error);
    }
};

// Test connection from configs
const testConnectionFromConfigs = async () => {
    try {
        await tryConnect({ useConfig: true });
        console.log("Connection successful!");
    } catch (error) {
        console.error("Connection test failed:", error);
    }
};

// Test connection with user input
const testConnection = async () => {
    try {
        if (!selectedDbType.value || !connectionString.value) {
            throw new Error("Database type and connection string are required.");
        }
        await tryConnect({
            dbType: selectedDbType.value,
            connectionString: connectionString.value,
        });
        console.log("Connection successful!");
    } catch (error) {
        console.error("Connection test failed:", error);
    }
};

// Save connection
const saveConnectionInfo = async () => {
    try {
        if (!selectedDbType.value || !connectionString.value) {
            throw new Error("Database type and connection string are required.");
        }
        await saveConnection({
            dbType: selectedDbType.value,
            connectionString: connectionString.value,
        });
        console.log("Connection saved!");
    } catch (error) {
        console.error("Failed to save connection:", error);
    }
};

// Lifecycle hooks
onMounted(loadConnectionSettings);
</script>

<style scoped>
.database-connection {
    max-width: 600px;
    margin: 0 auto;
}
</style>