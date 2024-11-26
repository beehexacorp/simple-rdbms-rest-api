<template>
  <a-card>
    <!-- Tabs for Entity Query and Raw Query -->
    <a-tabs v-model:activeKey="activeTab" style="margin-bottom: 20px">
      <a-tab-pane key="entity-query" tab="Entity Query">
        <div>
          <h3>Entity Query Example</h3>

          <!-- Fields Input -->
          <label for="fieldsInput" style="display: block; margin-bottom: 5px">
            Fields
            <a-tooltip>
              <template #title>
                <div>
                  <p>Example: one field per line</p>
                  <p v-for="field in defaultFields" :key="field">{{ field }}</p>
                </div>
              </template>
              <span>
                <QuestionCircleOutlined style="margin-left: 5px; color: rgba(0, 0, 0, 0.45)" />
              </span>
            </a-tooltip>
          </label>
          <a-textarea
            id="fieldsInput"
            v-model:value="entityQuery.fields"
            :placeholder="defaultFields.join('\r\n')"
            :rows="4"
            style="margin-bottom: 10px"
          />

          <!-- Conditions Input -->
          <label for="conditionsInput" style="display: block; margin-bottom: 5px">
            Conditions
            <a-tooltip>
              <template #title>
                <div>
                  <p>Example: one condition per line</p>
                  <p>column_a = @param_a</p>
                  <p>column_b = 'abc'</p>
                </div>
              </template>
              <span>
                <QuestionCircleOutlined style="margin-left: 5px; color: rgba(0, 0, 0, 0.45)" />
              </span>
            </a-tooltip>
          </label>
          <a-textarea
            id="conditionsInput"
            v-model:value="entityQuery.conditions"
            placeholder="Enter conditions"
            :rows="4"
            style="margin-bottom: 10px"
          />

          <!-- Order By Input -->
          <label for="orderByInput" style="display: block; margin-bottom: 5px">
            Order By
            <a-tooltip>
              <template #title>
                <div>
                  <p>Example:</p>
                  <p>column_a ASC</p>
                  <p>column_b DESC</p>
                </div>
              </template>
              <span>
                <QuestionCircleOutlined style="margin-left: 5px; color: rgba(0, 0, 0, 0.45)" />
              </span>
            </a-tooltip>
          </label>
          <a-input
            id="orderByInput"
            v-model:value="entityQuery.orderBy"
            placeholder="Enter Order By clause"
            style="margin-bottom: 10px; width: 300px"
          />
        </div>
      </a-tab-pane>

      <a-tab-pane key="raw-query" tab="Raw Query">
        <div>
          <h3>Raw Query Example</h3>
          <label for="rawQueryInput" style="display: block; margin-bottom: 5px">
            SQL Query
            <a-tooltip>
              <template #title>
                <div>
                  <p>Example:</p>
                  <p>SELECT {{ defaultFields.join(', ') }}</p>
                  <p>FROM {{ defaultTableName }}</p>
                  <p>LIMIT 10 OFFSET 0</p>
                </div>
              </template>
              <span>
                <QuestionCircleOutlined style="margin-left: 5px; color: rgba(0, 0, 0, 0.45)" />
              </span>
            </a-tooltip>
          </label>
          <a-textarea
            id="rawQueryInput"
            v-model:value="rawQuery.sql"
            :placeholder="generateSqlExample"
            :rows="8"
            style="margin-bottom: 10px"
          />
        </div>
      </a-tab-pane>
    </a-tabs>

    <!-- cURL and Translated API Request Tabs -->
    <a-tabs default-active-key="curl">
      <a-tab-pane key="curl" tab="cURL">
        <pre>{{ translatedCurl }}</pre>
      </a-tab-pane>
      <a-tab-pane key="api-request" tab="API Request">
        <div>
          <p><strong>Method:</strong> POST</p>
          <p><strong>Url:</strong> {{ translatedApiUrl }}</p>
          <p><strong>Body:</strong></p>
          <pre>{{ translatedApiBody }}</pre>
        </div>
      </a-tab-pane>
    </a-tabs>
  </a-card>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, defineProps } from 'vue'
import { QuestionCircleOutlined } from '@ant-design/icons-vue'
import { fetchConnectionInfo } from '@/services/connectionService'
import { Connection } from '@/types/Connection'

// Props
const props = defineProps({
  connectionId: {
    type: String,
    required: true,
  },
  tableColumns: {
    type: Array,
    required: true,
  },
  tableInfo: {
    type: Object,
    required: true,
  },
})

// Reactive state
const connectionInfo = ref<Connection | null>(null)
const activeTab = ref('entity-query')
const entityQuery = ref({
  tableName: '',
  fields: '',
  conditions: '',
  orderBy: '',
})
const rawQuery = ref({
  sql: '',
})

// Change paramsInput to a list
const paramsInput = ref<string[]>(["column_a='abc'", 'column_b=1'])

// Parse params input to JSON
const parsedParams = computed(() => {
  return paramsInput.value.reduce((acc: Record<string, string>, param: string) => {
    const [key, value] = param.split('=').map((s) => s.trim())
    acc[key] = value
    return acc
  }, {})
})

// Default table name and fields based on parsedParams and tableColumns
const defaultTableName = computed(() => props.tableInfo.table_name || 'sample_table')
const defaultFields = computed(() => props.tableColumns.map((col: any) => col.column_name))

// Generate SQL example based on DbType
const generateSqlExample = computed(() => {
  if (!connectionInfo.value) return 'Enter SQL query here...'
  const dbType = connectionInfo.value.dbType as any
  switch (dbType) {
    case 'SQL_SERVER':
      return `SELECT ${defaultFields.value.join(', ')} 
FROM ${defaultTableName.value} 
ORDER BY column_a ASC 
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY`
    case 'ORACLE':
      return `SELECT ${defaultFields.value.join(', ')} 
FROM ${defaultTableName.value} 
WHERE ROWNUM <= 10`
    case 'MYSQL':
      return `SELECT ${defaultFields.value.join(', ')} 
FROM ${defaultTableName.value} 
LIMIT 10 OFFSET 0`
    case 'POSTGRES':
      return `SELECT ${defaultFields.value.join(', ')} 
FROM ${defaultTableName.value} 
LIMIT 10 OFFSET 0`
    default:
      return 'Enter SQL query here...'
  }
})

// Translate inputs to cURL command
const translatedCurl = computed(() => {
  const url =
    activeTab.value === 'entity-query'
      ? `/api/entity/${props.connectionId}/query`
      : `/api/sql/${props.connectionId}/query`
  const body =
    activeTab.value === 'entity-query'
      ? {
          tableName: entityQuery.value.tableName || defaultTableName.value,
          fields: entityQuery.value.fields.split('\n'),
          conditions: entityQuery.value.conditions.split('\n'),
          orderBy: entityQuery.value.orderBy,
          params: parsedParams.value,
        }
      : {
          query: rawQuery.value.sql,
          params: parsedParams.value,
        }

  return `curl -X POST '${url}' \\
  -H 'Content-Type: application/json' \\
  -d '${JSON.stringify(body, null, 2)}'`
})

// Translate inputs to formatted API request body
const translatedApiUrl = computed(() => {
  return activeTab.value === 'entity-query' ? '/api/entity/query' : '/api/sql/query'
})

const translatedApiBody = computed(() => {
  return JSON.stringify(
    activeTab.value === 'entity-query'
      ? {
          tableName: entityQuery.value.tableName || defaultTableName.value,
          fields: entityQuery.value.fields.split('\n'),
          conditions: entityQuery.value.conditions.split('\n'),
          orderBy: entityQuery.value.orderBy,
          params: parsedParams.value,
        }
      : {
          query: rawQuery.value.sql,
          params: parsedParams.value,
        },
    null,
    2,
  )
})

// Watch activeTab and input changes to recompute outputs
watch(
  [activeTab, entityQuery, rawQuery, paramsInput],
  () => {
    // The computed properties (translatedCurl, translatedApiUrl, translatedApiBody) will automatically update
  },
  { deep: true },
)

// Fetch connection info on mounted
onMounted(async () => {
  try {
    connectionInfo.value = await fetchConnectionInfo(props.connectionId)
    rawQuery.value.sql = generateSqlExample.value
    entityQuery.value.fields = props.tableColumns
      .map((col: any) => col.column_name)
      .filter((x) => !x.startsWith('__'))
      .join('\n') //defaultFields.value
    console.log('Connection info fetched:', connectionInfo.value)
  } catch (error) {
    console.error('Failed to fetch connection info:', error)
  }
})
</script>

<style scoped>
.entity-detail {
  margin-bottom: 20px;
}
</style>
