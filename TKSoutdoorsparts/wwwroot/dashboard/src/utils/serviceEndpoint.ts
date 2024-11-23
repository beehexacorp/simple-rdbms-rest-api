export function useServiceEndpoint() {
  return {
    normalize(endpoint: string) {
      const apiUrl = import.meta.env.VITE_API_URL
        ? `${import.meta.env.VITE_API_URL}/${endpoint}`
        : `/${endpoint}`
      return apiUrl
    },
  }
}
