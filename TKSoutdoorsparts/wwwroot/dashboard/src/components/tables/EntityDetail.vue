<template>
  <a-card class="entity-detail" title="Entity Information">
    <!-- Entity Information -->
    <a-descriptions bordered :column="3">
      <a-descriptions-item
        v-for="[key, value] in Object.entries(tableInfo).filter(([key]) => !key.startsWith('__'))"
        :label="key"
        :key="key"
      >
        {{ value }}
      </a-descriptions-item>
    </a-descriptions>
  </a-card>
  <a-divider style="margin: 10px 0" />
  <a-card class="query-usage" title="Examples">
    <!-- Pass tableColumns to Query Usage -->
    <query-usage :table-columns="allColumns" :table-info="tableInfo" />
  </a-card>
  <a-divider style="margin: 10px 0" />
  <a-card class="entity-detail" title="Entity Columns">
    <!-- Search Input -->
    <div class="search-bar">
      <a-input-search
        v-model:value="searchQuery"
        placeholder="Search fields..."
        @input="filterData"
        style="width: 300px; margin-bottom: 16px"
      />
    </div>

    <!-- Details Table -->
    <div v-if="filteredDetails.length">
      <a-table
        :columns="tableColumns"
        :dataSource="paginatedDetails"
        :pagination="false"
        rowKey="id"
        bordered
      ></a-table>

      <!-- Pagination -->
      <div class="pagination" style="margin-top: 16px; text-align: center">
        <a-button :disabled="currentPage === 1" @click="changePage(currentPage - 1)">
          Previous
        </a-button>
        <span style="margin: 0 8px">Page {{ currentPage }} of {{ totalPages }}</span>
        <a-button :disabled="currentPage === totalPages" @click="changePage(currentPage + 1)">
          Next
        </a-button>
      </div>
    </div>
    <a-empty v-else description="No data available" />
  </a-card>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { getTableDetails } from '@/services/entityService'
import Fuse from 'fuse.js'
import { useRoute } from 'vue-router'

// Query the "detail" parameter from the route
const route = useRoute()
const detailEncoded = route.query.detail

// Reactive state
const tableInfo = ref({})
const allColumns = ref([])
const filteredDetails = ref([])
const searchQuery = ref('')
const currentPage = ref(1)
const itemsPerPage = 10

// Computed properties
const totalPages = computed(() => Math.ceil(filteredDetails.value.length / itemsPerPage))

const paginatedDetails = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredDetails.value.slice(start, end)
})

// Default columns based on filtered details
const tableColumns = computed(() => {
  if (filteredDetails.value.length === 0) return []
  const keys = Object.keys(filteredDetails.value[0])
  return keys.map((key) => ({
    title: key,
    dataIndex: key,
    key,
  }))
})

// Decode and parse the detail parameter
const decodeDetail = () => {
  if (!detailEncoded) {
    throw new Error('Detail parameter is missing')
  }

  try {
    const decodedString = atob(detailEncoded)
    tableInfo.value = JSON.parse(decodedString)
  } catch (error) {
    console.error('Failed to decode or parse detail:', error)
  }
}

// Fetch details from the API
const fetchDetails = async () => {
  try {
    if (!detailEncoded) {
      throw new Error('Detail parameter is missing')
    }

    const data = await getTableDetails(detailEncoded)
    allColumns.value = data
    filteredDetails.value = allColumns.value
  } catch (error) {
    console.error(error)
  }
}

// Filter data using Fuse.js
const filterData = () => {
  if (!searchQuery.value.trim()) {
    filteredDetails.value = allColumns.value
  } else {
    const fuse = new Fuse(allColumns.value, {
      keys: Object.keys(allColumns.value[0] || {}),
      threshold: 0.4, // Allow typo-tolerant matches
      distance: 100, // Increase flexibility for matches
      minMatchCharLength: 1,
    })
    filteredDetails.value = fuse.search(searchQuery.value).map((result) => result.item)
  }
  currentPage.value = 1
}

// Change page for pagination
const changePage = (page) => {
  currentPage.value = page
}

// Decode and fetch details on component mount
onMounted(() => {
  decodeDetail()
  fetchDetails()
})
</script>

<style scoped>
.entity-detail {
}

.search-bar {
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
