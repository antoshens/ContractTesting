echo "This is just an example of usage. Please change the values in brackets <> to your actual data."

docker run --rm --network="host" -v "//<absolute_path_to_the_project_root>/pacts":/run/desktop/mnt/host/pacts pactfoundation/pact-cli:latest publish /run/desktop/mnt/host/pacts -b="http://localhost:9292" -u="postgres" -p="password" -h=master --merge --consumer-app-version %RANDOM%
