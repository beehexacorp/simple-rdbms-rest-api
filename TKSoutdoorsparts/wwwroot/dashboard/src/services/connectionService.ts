import { useServiceEndpoint } from '@/utils/serviceEndpoint'

export interface ConnectionInfoViewModel {
  DbType: number
  Database: string
  Host: string
  Port: string
  User: string
}

export interface TestConnectionRequest {
  dbType?: number
  connectionString?: string
  useConfig?: boolean // When true, it tests the connection using existing configs
}

const serviceEndpointHandler = useServiceEndpoint()

/**
 * Fetch the current connection info.
 * @returns ConnectionInfoViewModel or null if no connection info is available.
 */
export const fetchConnectionInfo = async (): Promise<ConnectionInfoViewModel | null> => {
  const apiUrl = serviceEndpointHandler.normalize('api/connection')
  const response = await fetch(apiUrl, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  })

  if (response.status === 204) {
    // No content, indicating no connection info available
    return null
  }

  if (!response.ok) {
    throw new Error(`Error fetching connection info: ${response.statusText}`)
  }

  return await response.json()
}

/**
 * Fetch the database types.
 * @returns An array of database type options.
 */
export const fetchDbTypes = async (): Promise<{ value: number; label: string }[]> => {
  const apiUrl = serviceEndpointHandler.normalize('api/connection/db-type')
  const response = await fetch(apiUrl, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  })

  if (!response.ok) {
    throw new Error(`Error fetching database types: ${response.statusText}`)
  }

  const data = await response.json()
  return Object.entries(data).map(([key, value]) => ({
    value: parseInt(key, 10),
    label: value as string,
  }))
}

/**
 * Test the database connection.
 * @param request The connection test request.
 */
export const tryConnect = async (request: TestConnectionRequest): Promise<void> => {
  const apiUrl = serviceEndpointHandler.normalize(
    request.useConfig ? 'api/connection/connect-from-configs' : 'api/connection/connect',
  )

  const response = await fetch(apiUrl, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      DbType: request.dbType,
      ConnectionString: request.connectionString,
    }),
  })

  if (!response.ok) {
    throw new Error(`Error testing connection: ${response.statusText}`)
  }
}

/**
 * Save the database connection.
 * @param request The connection info to save.
 */
export const saveConnection = async (request: TestConnectionRequest): Promise<void> => {
  const apiUrl = serviceEndpointHandler.normalize('api/connection')
  const response = await fetch(apiUrl, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      DbType: request.dbType,
      ConnectionString: request.connectionString,
    }),
  })

  if (!response.ok) {
    throw new Error(`Error saving connection: ${response.statusText}`)
  }
}