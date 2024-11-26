import type { Connection } from '@/types/Connection'
import { useServiceEndpoint } from '@/utils/serviceEndpoint'

const serviceEndpointHandler = useServiceEndpoint()

export interface TestConnectionRequest {
  dbType?: number
  connectionString?: string
  connectonId?: string // When true, it tests the connection using existing configs
}

export interface SaveConnectionRequest {
  dbType: number
  connectionString: string
}

/**
 * Fetch the current connection info.
 * @returns Connection[] or null if no connection info is available.
 */
export const fetchConnectionInfos = async (): Promise<Connection[] | null> => {
  const apiUrl = serviceEndpointHandler.normalize(`api/connection`)
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
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
  }

  return await response.json()
}

/**
 * Fetch the current connection info.
 * @returns Connection or null if no connection info is available.
 */
export const fetchConnectionInfo = async (connectionId: string): Promise<Connection | null> => {
  const apiUrl = serviceEndpointHandler.normalize(`api/connection/${connectionId}`)
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
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
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
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
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
    !!request.connectonId
      ? `api/connection/${request.connectonId}/connect`
      : 'api/connection/connect',
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
    const { errorMessage, errors } = await response.json()
    if (errors && Object.keys(errors).length > 0) {
      throw new Error(errors[Object.keys(errors)[0]])
    } else {
      throw new Error(errorMessage)
    }
  }
}

/**
 * Save the database connection.
 * @param request The connection info to save.
 */
export const saveConnection = async (request: SaveConnectionRequest): Promise<void> => {
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
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
  }
}
