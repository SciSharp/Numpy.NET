# Numpy.NET
Numpy.NET brings the awesome Python package [NumPy](https://www.numpy.org/) to the .NET world. NumPy is THE fundamental library for scientific computing, machine learning and AI in Python. Numpy.NET empowers .NET developers to leverage NumPy's extensive functionality including multi-dimensional arrays and matrices, linear algebra, FFT and many more via a compatible strong typed API. 

## Example

## How does it work?

Numpy.NET uses [Python for .NET](http://pythonnet.github.io/) to call into the Python module `numpy`. However, this does not mean that it depends on a local Python installation! Numpy.NET.dll uses [Python.Included](https://github.com/henon/Python.Included) which packages embedded Python 3.7 and automatically deploys it in the user's home directory upon first execution. On subsequent runs, it will find Python already deployed and therefor doesn't install it again. Numpy.NET also packages the NumPy wheel and installs it into the embedded Python installation when not yet installed. 

Long story short: as a .NET Developer **you don't need to worry about Python** at all. You just reference Numpy.NET, use it and **it will just work**, no matter if you have local Python installations or not.

## Performance considerations

You might ask how calling into Python affects performance. As always, it depends on your usage. Don't forget that `numpy`'s number crunching algorithms are written in `C` so the thin `pythonnet` and `Python` layers on top won't have a significant impact if you are working with larger amounds of data. 

All of `numpy` is centered around the `ndarray` class which allows you to pass a huge chunk of data into the `C` routines and let them execute all kinds of operations on them efficiently. So if you are manipulating arrays or matrices with thousands or hundreds of thousands of elements, the overhead will be neglegible.

*Array creation* actually has the most impact on performance since the data has to be moved from the `CLR` into the `Python` interpreter. `pythonnet` does not optimize for passing large arrays from `C#` to `Python` but we still found a way to do that very efficiently. When creating an array with `np.array( ... )` we internally use `Marshal.Copy` to copy the entiry `C#`-array's memory into the `numpy`-array's storage. And to efficiently retrieve computation results from `numpy` there is a method called `GetData<T>` which will copy the data back to `C#`.

## Numpy.NET vs NumSharp

The SciSharp team is also developing a pure C# port of NumPy called [NumSharp](https://github.com/SciSharp/NumSharp) which is quite popular albeit incomplete. To help you decide which one to use we compare the advantages and disadvantages of both libraries here:

| Aspect        | Numpy.NET                             | NumSharp      |
| ------------- | ------------------------------------- | ------------- |
| Dependencies  | CPython / NumPy                       | C++ dlls for certain operations |
| Setup         | Reference Nuget-Package               | Reference Nuget-Package |
| Completeness  | Large parts are wrapped               | A small subset of most important functions is ported |
| Development   | Fast, due to automated API generation | Very slow, due to lack of manpower |
| Correctness   | Same results as in Python guaranteed  | There are many subtle differences |
| Actuality     | Can easily keep up with `numpy` dev   | Will always trail way behind, due to lack of manpower |
| GPU support   | None                                  | Using a GPU backend for calculatons possible, per design |
| Performance   | TODO: measure                         | TODO: measure |

## Code generation

The vast majority of Numpy.NET's code is generated using [CodeMinion](https://github.com/SciSharp/CodeMinion) by parsing the documentation at [docs.scipy.org/doc/numpy/](docs.scipy.org/doc/numpy/). This allowed us to wrap most of the `numpy`-API in just two weeks. The rest of the API can be completed in a few more weeks if there is popular demand. 

## API Completion

TODO: information about completed API categories

## Versions and Compatibility

Currently, Numpy.NET is targeting .NET Standard (on Windows) and packages the following binaries:
* Python 3.7: (python-3.7.3-embed-amd64.zip)
* NumPy 1.16 (numpy-1.16.3-cp37-cp37m-win_amd64.whl)

To make Numpy.NET support Linux a separate version of [Python.Included]() packaging linux binaries of Python needs to be made and a version of Numpy.NET that packages a linux-compatible NumPy wheel. 

## License

Numpy.NET packages and distributes `Python`, `pythonnet` as well as `numpy`. All these dependencies imprint their license conditions upon Numpy.NET. The C# wrapper itself is MIT License. 



